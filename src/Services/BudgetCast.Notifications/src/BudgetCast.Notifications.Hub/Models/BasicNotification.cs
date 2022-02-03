namespace BudgetCast.Notifications.AppHub.Models
{
    public record BasicNotification : INotificationMessage
    {
        public string MessageType { get; init; } = nameof(BasicNotification);

        public string? Message { get; init; }

        public NotificationType Label { get; init; }
    }
}
