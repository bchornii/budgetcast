namespace BudgetCast.Common.Domain;

public class DomainEventNotificationBase : IDomainEventNotification
{
    public DateTime AddedAt { get; }

    public DomainEventNotificationBase()
    {
        AddedAt = SystemDt.Current;
    }
}