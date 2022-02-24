using BudgetCast.Common.Messaging.Abstractions.Handlers;

namespace BudgetCast.Common.Messaging.Abstractions.Messages.Events
{
    public interface IEventSubscriptionManager
    {
        bool HasNoSubscriptions { get; }
        event EventHandler<string> OnEventRemoved;

        void AddSubscription<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>;

        void RemoveSubscription<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>;

        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IReadOnlyList<SubscriptionInformation> GetHandlersForEvent<T>() where T : IntegrationEvent;
        IReadOnlyList<SubscriptionInformation> GetHandlersForEvent(string eventName);
        string GetEventKey<T>();
    }
}
