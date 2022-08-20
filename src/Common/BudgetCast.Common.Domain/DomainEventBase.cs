namespace BudgetCast.Common.Domain;

public class DomainEventBase : IDomainEvent
{
    public DateTime OccuredOn { get; }

    public DomainEventBase()
    {
        OccuredOn = SystemDt.Current;
    }
}