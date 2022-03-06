namespace BudgetCast.Common.Messaging.Abstractions.Events;

public interface IEventsPublisher
{
    Task<bool> Publish(IntegrationEvent @event, CancellationToken cancellationToken);
}