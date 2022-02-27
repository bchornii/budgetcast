namespace BudgetCast.Common.Messaging.Abstractions.Events
{
    public interface IEventsSubscriptionManager
    {
        bool HasNoSubscriptions { get; }
        event EventHandler<string> OnEventRemoved;

        void AddSubscription<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IEventHandler<TEvent>;

        void RemoveSubscription<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IEventHandler<TEvent>;

        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IReadOnlyList<EventSubscriptionInformation> GetHandlersForEvent<T>() where T : IntegrationEvent;
        IReadOnlyList<EventSubscriptionInformation> GetHandlersForEvent(string eventName);
        string GetEventKey<T>();
    }
}
