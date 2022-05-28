using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using BudgetCast.Common.Web.Logs;
using BudgetCast.Expenses.Messaging;
using BudgetCast.Notifications.AppHub.EventHandlers;
using Serilog;

namespace BudgetCast.Notifications.AppHub.Infrastructure.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddWebAppHostConfiguration(this IHostBuilder hostBuilder)
        => hostBuilder
            .UseSharedSerilogConfiguration()
            .UseAzureServiceBus(
                registerHandlers: services =>
                {
                    services.AddScoped<ExpensesEventsHandler>();
                },
                subscribeToEvents: processor =>
                {
                    processor.SubscribeTo<ExpensesAddedEvent, ExpensesEventsHandler>();
                    processor.SubscribeTo<ExpensesRemovedEvent, ExpensesEventsHandler>();
                });
}