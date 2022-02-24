using BudgetCast.Common.Messaging.Abstractions.Messages.Events;

namespace BudgetCast.Common.Messaging.Abstractions.Handlers
{
    public interface IIntegrationEventHandler<in TEvent> : IIntegrationMessageHandler
        where TEvent : IntegrationEvent
    {
        Task Handle(TEvent @event, CancellationToken cancellationToken);
    }
}
