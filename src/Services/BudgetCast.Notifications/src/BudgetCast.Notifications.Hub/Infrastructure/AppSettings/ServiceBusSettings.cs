namespace BudgetCast.Notifications.AppHub.Infrastructure.AppSettings
{
    public class ServiceBusSettings
    {
        public bool AzureServiceBusEnabled { get; set; }

        public string EventBusConnection { get; set; }
    }
}
