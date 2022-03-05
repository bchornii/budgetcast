using System.Collections.Generic;

namespace BudgetCast.Common.Messaging.Abstractions.Events
{
    public interface IEventsSubscriptionManager
    {
        /// <summary>
        /// Returns <c>true</c> if no subscriptions has been created yet.
        /// </summary>
        bool HasNoSubscriptions { get; }
        
        /// <summary>
        /// Event triggered when all subscriptions for particular event type have been removed. 
        /// </summary>
        event EventHandler<string> OnEventRemoved;

        /// <summary>
        /// Adds subscription to integration event.
        /// </summary>
        /// <typeparam name="TEvent">Integration event type</typeparam>
        /// <typeparam name="THandler">Integration event handler type</typeparam>
        void AddSubscription<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IEventHandler<TEvent>;

        /// <summary>
        /// Removes subscription from integration event.
        /// </summary>
        /// <typeparam name="TEvent">Integration event type</typeparam>
        /// <typeparam name="THandler">Integration event handler type</typeparam>
        void RemoveSubscription<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IEventHandler<TEvent>;
        
        /// <summary>
        /// Removes all subscriptions.
        /// </summary>
        void RemoveAll();

        /// <summary>
        /// Verifies if any subscription for event of type <see cref="T"/> have been added.
        /// </summary>
        /// <typeparam name="T">Integration event type</typeparam>
        /// <returns></returns>
        bool HasSubscriptionsForEvent<T>() 
            where T : IntegrationEvent;
        
        /// <summary>
        /// Verifies if any subscription for event name have been added.
        /// </summary>
        /// <param name="eventName">Integration event name</param>
        /// <returns></returns>
        bool HasSubscriptionsForEvent(string eventName);
        
        /// <summary>
        /// Retrieves integration event type from name.
        /// </summary>
        /// <param name="eventName">Integration event name</param>
        /// <returns></returns>
        Type GetEventTypeByName(string eventName);

        /// <summary>
        /// Retrieves <see cref="IReadOnlyCollection{T}"/> of integration event handlers. 
        /// </summary>
        /// <typeparam name="T">Integration event name</typeparam>
        /// <returns></returns>
        IReadOnlyList<EventSubscriptionInformation> GetHandlersForEvent<T>() 
            where T : IntegrationEvent;
        
        /// <summary>
        /// Retrieves <see cref="IReadOnlyCollection{T}"/> of integration event handlers.
        /// </summary>
        /// <param name="eventName">Integration event name</param>
        /// <returns></returns>
        IReadOnlyList<EventSubscriptionInformation> GetHandlersForEvent(string eventName);
    }
}
