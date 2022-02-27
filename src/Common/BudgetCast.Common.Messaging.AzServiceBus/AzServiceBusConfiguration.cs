namespace BudgetCast.Common.Messaging.AzServiceBus;

public class AzServiceBusConfiguration
{
    public string SubscriptionClientName { get; set; } = default!;

    public string AzureServiceBusConnectionString { get; set; } = default!;
}