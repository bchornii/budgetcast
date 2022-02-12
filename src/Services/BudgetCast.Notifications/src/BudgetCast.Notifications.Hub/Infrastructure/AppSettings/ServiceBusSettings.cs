namespace BudgetCast.Notifications.AppHub.Infrastructure.AppSettings
{
    public class ServiceBusSettings
    {
        public ServiceBusSettings()
        {
            EventBusConnection = null!;
        }
        
        public bool AzureServiceBusEnabled { get; set; }

        public string EventBusConnection { get; set; }
    }
}
