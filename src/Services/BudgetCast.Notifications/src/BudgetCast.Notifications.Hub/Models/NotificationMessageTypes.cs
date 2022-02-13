namespace BudgetCast.Notifications.AppHub.Models
{
    public static class NotificationMessageTypes
    {
        public const string Unknown = nameof(Unknown);

        public const string ExpensesAdded = nameof(ExpensesAdded);
        public const string ExpensesRemoved = nameof(ExpensesRemoved);

        public const string CampaignAdded = nameof(CampaignAdded);
        public const string CampaignJoined = nameof(CampaignJoined);
    }
}
