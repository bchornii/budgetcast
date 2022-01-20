using BudgetCast.Identity.Api.HostedServices;
using BudgetCast.Identity.Api.Infrastructure.AppSettings;
using BudgetCast.Identity.Api.Infrastructure.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace BudgetCast.Identity.Api.Infrastructure.Extensions
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

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
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
                options.AddDefaultPolicy(builder =>
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
                });

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Budget Cast Identity API", Version = "v1" });
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

        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

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
    }
}
