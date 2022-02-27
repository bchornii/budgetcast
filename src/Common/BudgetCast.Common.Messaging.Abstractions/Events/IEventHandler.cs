using BudgetCast.Common.Messaging.Abstractions.Common;

namespace BudgetCast.Common.Messaging.Abstractions.Events
{
    public interface IEventHandler<in TEvent> : IMessageHandler
        where TEvent : IntegrationEvent
    {
        Task Handle(TEvent @event, CancellationToken cancellationToken);
    }
}
