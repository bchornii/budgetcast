namespace BudgetCast.Common.Messaging.Azure.ServiceBus;

public class ServiceBusConfiguration
{
    public string SubscriptionClientName { get; set; } = default!;

    public string AzureServiceBusConnectionString { get; set; } = default!;
}