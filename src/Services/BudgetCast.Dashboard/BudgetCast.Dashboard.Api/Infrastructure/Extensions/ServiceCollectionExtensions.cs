using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Api.HostedServices;
using BudgetCast.Dashboard.Api.Infrastructure.AppSettings;
using BudgetCast.Dashboard.Api.Infrastructure.Extensions;
using BudgetCast.Dashboard.Api.Infrastructure.Services;
using BudgetCast.Dashboard.Data;
using FluentValidation.AspNetCore;
using MediatR;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BudgetCast.Dashboard.Api.Infrastructure.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomConfigSections(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.UseConfigurationValidation();

            services.ConfigureValidatableSetting<EmailParameters>(
                configuration.GetSection("EmailParameters"));

            services.ConfigureValidatableSetting<UiLinks>(
                configuration.GetSection("UiLinks"));

            services.ConfigureValidatableSetting<ExternalIdentityProviders>(
                configuration.GetSection("ExternalIdentityProviders"));

            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<EmailService>();
            return services;
        }

        public static IServiceCollection AddCustomMvc(this IServiceCollection services,
            IConfiguration configuration)
        {
            var uiRoot = configuration["UiLinks:Root"];
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder
                        .WithOrigins(uiRoot)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddControllers()
                .AddFluentValidation(options =>
                {
                    options.RegisterValidatorsFromAssemblyContaining<Startup>();
                })
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver());

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Budget Cast API", Version = "v1" });
            });

            return services;
        }

        public static IServiceCollection AddAspNetIdentity(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseSqlServer(configuration["IdentityManagement:ConnectionString"],
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(
                            typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 15,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            });

            services
                .AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireDigit = true;

                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Domain = configuration["ParentDomain"];
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;

                options.Events.OnRedirectToLogin = redirectContext =>
                {
                    redirectContext.HttpContext.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToAccessDenied = redirectContext =>
                {
                    redirectContext.HttpContext.Response.StatusCode = 403;
                    return Task.CompletedTask;
                };
            });

            return services;
        }

        public static IServiceCollection AddAuthentication(this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
                })
                .AddGoogle(configuration["ExternalIdentityProviders:Google:Name"],
                    options =>
                    {
                        options.CallbackPath = new PathString("/g-callback");
                        options.ClientId = configuration["Social:Google:ClientId"];
                        options.ClientSecret = configuration["Social:Google:ClientSecret"];
                    })
                .AddFacebook(configuration["ExternalIdentityProviders:Facebook:Name"],
                    options =>
                    {
                        options.CallbackPath = new PathString("/fb-callback");
                        options.ClientId = configuration["Social:Facebook:ClientId"];
                        options.ClientSecret = configuration["Social:Facebook:ClientSecret"];
                    });

            return services;
        }

        public static IServiceCollection AddMongoContext(this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddMongoMaps()
                .AddScoped(serviceProvider =>
            {
                var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
                var userId = httpContextAccessor?.HttpContext?.GetUserId();
                var connectionString = configuration["BudgetCast:ConnectionString"];
                return new BudgetCastContext(connectionString, serviceProvider.GetService<IMediator>(), userId);
            });

            return services;
        }

        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            hcBuilder
                .AddMongoDb(
                    configuration["BudgetCast:ConnectionString"],
                    name: "BudgetCast-MongoDb-Check",
                    tags: new string[] { "mongodb" });

            hcBuilder
                .AddSqlServer(
                    configuration["IdentityManagement:ConnectionString"],
                    name: "IdentityManagement-SqlDb-Check",
                    tags: new string[] { "catalogdb" });

            return services;
        }

        public static IServiceCollection AddCustomHostedServices(this IServiceCollection services)
        {
            services
                .AddHostedService<IdentityDbMigrationHostedService>();

            return services;
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper();
            return services;
        }
    }
}
