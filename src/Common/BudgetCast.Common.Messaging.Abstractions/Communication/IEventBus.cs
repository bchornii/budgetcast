using BudgetCast.Common.Messaging.Abstractions.Handlers;
using BudgetCast.Common.Messaging.Abstractions.Messages.Events;

namespace BudgetCast.Common.Messaging.Abstractions.Communication
{
    public interface IEventBus : IMessagingBus
    {
        Task Publish(IntegrationEvent @event, CancellationToken cancellationToken);

        Task Subscribe<TEvent, THandler>(CancellationToken cancellationToken)
            where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>;

        Task Unsubscribe<TEvent, THandler>(CancellationToken cancellationToken)
            where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>;

        Task Start(CancellationToken cancellationToken);

        Task Stop(CancellationToken cancellationToken);
    }
}
