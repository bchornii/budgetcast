namespace BudgetCast.Notifications.AppHub.Models
{
    public interface INotificationMessage
    {
        public string MessageType { get; }

        public string? Message { get; }
        
        public NotificationType Type { get; }
    }
}
