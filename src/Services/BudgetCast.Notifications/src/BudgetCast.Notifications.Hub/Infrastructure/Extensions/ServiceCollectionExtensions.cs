using BudgetCast.Notifications.AppHub.Infrastructure.AppSettings;
using BudgetCast.Notifications.AppHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;
using System.Text;
using ILogger = Serilog.ILogger;

namespace BudgetCast.Notifications.AppHub.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services,
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

            return services;
        }

        public static IServiceCollection AddCustomSignalR(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            ILogger logger = Log.ForContext(typeof(Startup));

            var signalRSettings = configuration
                .GetSection(nameof(SignalRSettings)).Get<SignalRSettings>();

            if (!signalRSettings.UseBackplane)
            {
                services.AddSignalR();
            }
            else
            {
                var backplaneSettings = configuration
                    .GetSection("SignalRSettings:Backplane")
                    .Get<SignalRSettings.Backplane>();

                if (backplaneSettings is null)
                {
                    throw new InvalidOperationException("Backplane enabled, but no backplane settings in config.");
                }

                switch (backplaneSettings.Provider)
                {
                    case "redis":
                        
                        if (backplaneSettings.StringConnection is null)
                        {
                            throw new InvalidOperationException("Redis backplane provider: No connectionString configured.");
                        }

                        services
                            .AddSignalR()
                            .AddStackExchangeRedis(backplaneSettings.StringConnection, options =>
                            {
                                options.Configuration.AbortOnConnectFail = false;
                            });
                        break;

                    default:
                        throw new InvalidOperationException($"SignalR backplane Provider {backplaneSettings.Provider} is not supported.");
                }

                logger.Information($"SignalR Backplane Current Provider: {backplaneSettings.Provider}.");
            }

            return services;
        }

        public static IServiceCollection AddCustomHealthCheck(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            var signalRSettings = configuration.GetSection(nameof(SignalRSettings)).Get<SignalRSettings>();
            var serviceBusSettings = configuration.GetSection(nameof(ServiceBusSettings)).Get<ServiceBusSettings>();

            ILogger logger = Log.ForContext(typeof(Startup));

            var hcBuilder = services.AddHealthChecks();
            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            if (signalRSettings.UseBackplane)
            {
                var backplaneSettings = configuration
                    .GetSection("SignalRSettings:Backplane")
                    .Get<SignalRSettings.Backplane>();

                if (backplaneSettings is null)
                {
                    throw new InvalidOperationException("Backplane enabled, but no backplane settings in config.");
                }

                hcBuilder
                    .AddRedis(
                        redisConnectionString: backplaneSettings.StringConnection,
                        name: "signalr-redis-check",
                        tags: new string[] { "redis" });

                logger.Information("Enabled health checks for SignalR backplane (Redis)");
            }

            if (serviceBusSettings.AzureServiceBusEnabled)
            {
                hcBuilder
                    .AddAzureServiceBusTopic(
                        serviceBusSettings.EventBusConnection,
                        topicName: "eshop_event_bus",
                        name: "signalr-servicebus-check",
                        tags: new string[] { "servicebus" });

                logger.Information("Enabled health checks for Azure Service Bus");
            }
            else
            {
                //hcBuilder
                //    .AddRabbitMQ(
                //        $"amqp://{serviceBusSettings.EventBusConnection}",
                //        name: "signalr-rabbitmqbus-check",
                //        tags: new string[] { "rabbitmqbus" });

                logger.Information("Enabled health checks for RabbitMq");
            }

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection($"SecuritySettings:{nameof(JwtSettings)}").Get<JwtSettings>();

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
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key)),

                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,

                        RequireExpirationTime = true,
                        ValidateLifetime = true,

                        RequireSignedTokens = true,
                        RequireAudience = true,

                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.Name,

                        ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date                        
                    };
                    bearer.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/hubs"))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddTransient<INotificationService, NotificationService>();

            return services;
        }
    }
}
