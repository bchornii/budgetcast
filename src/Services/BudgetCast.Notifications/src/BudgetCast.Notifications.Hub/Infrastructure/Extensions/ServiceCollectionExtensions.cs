using BudgetCast.Notifications.AppHub.Hubs;
using BudgetCast.Notifications.AppHub.Infrastructure.AppSettings;
using BudgetCast.Notifications.AppHub.Infrastructure.HubFilters;
using BudgetCast.Notifications.AppHub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;
using System.Text;
using BudgetCast.Common.Application.Extensions;
using BudgetCast.Common.Operations;
using BudgetCast.Common.Web.Extensions;
using ILogger = Serilog.ILogger;

namespace BudgetCast.Notifications.AppHub.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Populates all required by the app services.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddWebAppServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddApplicationServices(
                operationRegistryType: typeof(InMemoryOperationRegistry),
                commandAndQueryAssemblies: typeof(Program).Assembly)
            .AddCustomCors(configuration)
            .AddCustomSignalR(configuration)
            .AddCustomHealthCheck(configuration)
            .AddJwtAuthentication(configuration)
            .AddCustomServices()
            .AddIdentityContext()
            .AddHttpContextAccessor()
            .AddMessagingExtensions()
            .AddOperationContext();
        
        return services;
    }
    
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
        ILogger logger = Log.ForContext(typeof(Program));

        var signalRSettings = configuration
            .GetSection(nameof(SignalRSettings)).Get<SignalRSettings>();

        if (signalRSettings.UseAzureSignalR)
        {
            services
                .AddSignalR(hubOptions =>
                {
                    hubOptions.AddFilter(new AuthInformationFilter());
                    hubOptions.AddFilter(new CurrentTenantFilter());
                })
                .AddAzureSignalR(options =>
                {
                    options.GracefulShutdown.Mode = Microsoft.Azure.SignalR.GracefulShutdownMode.WaitForClientsClose;
                    options.GracefulShutdown.Timeout = TimeSpan.FromSeconds(30);
                    options.GracefulShutdown.Add<NotificationHub>(async context =>
                    {
                        await context.Clients.All.SendAsync("exit");
                    });
                });

            logger.Information("SignalR is using Azure SignalR service");
        }
        else
        {
            services
                .AddSignalR(hubOptions =>
                {
                    hubOptions.AddFilter(new CurrentTenantFilter());
                });

            logger.Information("SignalR is in self-hosting mode");
        }

        return services;
    }

    public static IServiceCollection AddCustomHealthCheck(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var serviceBusSettings = configuration
            .GetSection(nameof(ServiceBusSettings)).Get<ServiceBusSettings>();

        ILogger logger = Log.ForContext(typeof(Program));

        var hcBuilder = services.AddHealthChecks();
        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        if (serviceBusSettings.AzureServiceBusEnabled)
        {
            hcBuilder
                .AddAzureServiceBusTopic(
                    serviceBusSettings.EventBusConnection,
                    topicName: "budgetcast_events_topic",
                    name: "notifications-servicebus-check",
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