using BudgetCast.Common.Messaging.Abstractions.Common;

namespace BudgetCast.Common.Messaging.Abstractions.Events
{
    public interface IEventsProcessor
    {
        Task Start(CancellationToken cancellationToken);

        Task Stop(CancellationToken cancellationToken);
        
        Task SubscribeTo<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IEventHandler<TEvent>;

        Task UnsubscribeFrom<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IEventHandler<TEvent>;
    }
}
