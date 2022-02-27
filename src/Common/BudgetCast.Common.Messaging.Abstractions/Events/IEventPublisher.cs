namespace BudgetCast.Common.Messaging.Abstractions.Events;

public interface IEventPublisher
{
    Task Publish(IntegrationEvent @event, CancellationToken cancellationToken);
}