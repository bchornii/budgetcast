namespace BudgetCast.Notifications.AppHub.Models
{
    public interface INotificationMessage
    {
        public string MessageType { get; set; }

        public string? Message { get; set; }
    }
}
