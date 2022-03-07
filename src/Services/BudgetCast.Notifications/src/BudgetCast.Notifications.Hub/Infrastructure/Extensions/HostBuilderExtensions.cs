using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using BudgetCast.Notifications.AppHub.EventHandlers;
using Serilog;

namespace BudgetCast.Notifications.AppHub.Infrastructure.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddWebAppHostConfiguration(this IHostBuilder hostBuilder)
        => hostBuilder
            .UseSerilog((ctx, services, config) =>
                config.ReadFrom.Configuration(ctx.Configuration))
            .UseAzureServiceBus(
                registerHandlers: services =>
                {
                    services.AddScoped<TestIntegrationEventHandler>();
                },
                subscribeToEvents: processor =>
                {
                    processor.SubscribeTo<TestIntegrationEvent, TestIntegrationEventHandler>();
                });
}