namespace BudgetCast.Common.Messaging.Abstractions.Events;

public interface IEventsPublisher
{
    Task Publish(IntegrationEvent @event, CancellationToken cancellationToken);
}