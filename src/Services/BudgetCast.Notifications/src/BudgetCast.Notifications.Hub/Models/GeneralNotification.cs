namespace BudgetCast.Notifications.AppHub.Models
{
    public record GeneralNotification : INotificationMessage
    {
        public string Target => nameof(GeneralNotification);

        public string MessageType { get; init; } 
            = NotificationMessageTypes.Unknown;

        public string? Message { get; init; }

        public NotificationType Type { get; init; }

        public dynamic? Data { get; init; }
    }
}
