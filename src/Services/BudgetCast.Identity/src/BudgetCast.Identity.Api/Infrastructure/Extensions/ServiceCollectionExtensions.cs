﻿using BudgetCast.Identity.Api.Database;
using BudgetCast.Identity.Api.Database.Models;
using BudgetCast.Identity.Api.HostedServices;
using BudgetCast.Identity.Api.Infrastructure.AppSettings;
using BudgetCast.Identity.Api.Infrastructure.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using System.Text;

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
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
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

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppIdentityContext>(options =>
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
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireDigit = true;

                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<AppIdentityContext>()
                .AddDefaultTokenProviders();            

            return services;
        }

        public static IServiceCollection AddCookieAuthentication(this IServiceCollection services,
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

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection($"SecuritySettings:{nameof(JwtSettings)}").Get<JwtSettings>();
            services.AddSingleton(jwtSettings);

            if (string.IsNullOrEmpty(jwtSettings.Key))
            {
                throw new InvalidOperationException("No Key defined in JwtSettings config.");
            }

            services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    bearer.RequireHttpsMetadata = true;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key)),

                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,

                        ValidateLifetime = true,

                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.Name,

                        ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date                        
                    };
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
