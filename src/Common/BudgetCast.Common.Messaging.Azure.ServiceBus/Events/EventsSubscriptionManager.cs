using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Events;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Events
{
    public class EventsSubscriptionManager : IEventsSubscriptionManager
    {
        private readonly ILogger<EventsSubscriptionManager> _logger;
        private readonly Dictionary<string, List<EventSubscriptionInformation>> _handlers;
        private readonly List<Type> _eventTypes;

        public bool HasNoSubscriptions => _handlers is { Count: 0 };

        public event EventHandler<string> OnEventRemoved;

        public EventsSubscriptionManager(ILogger<EventsSubscriptionManager> logger)
        {
            _logger = logger;
            _handlers = new Dictionary<string, List<EventSubscriptionInformation>>();
            _eventTypes = new List<Type>();

            OnEventRemoved = (_, __) => { };
        }

        public void AddSubscription<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IEventHandler<TEvent>
        {
            var eventName = GetEventKey<TEvent>();

            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<EventSubscriptionInformation>());
            }

            var handlerType = typeof(THandler);
            if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered " +
                    $"for '{eventName}'", nameof(handlerType));
            }

            _handlers[eventName]
                .Add(new EventSubscriptionInformation(handlerType));

            if (!_eventTypes.Contains(typeof(TEvent)))
            {
                _eventTypes.Add(typeof(TEvent));
            }

            _logger.LogInformationIfEnabled(
                "Subscription added: {EventHandler} handler subscribed to {EventType} event",
                handlerType.Name,
                eventName);
        }

        public void RemoveSubscription<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IEventHandler<TEvent>
        {
            var subscriptionInformation = FindSubscriptionToRemove<TEvent, THandler>();

            if (subscriptionInformation == EventSubscriptionInformation.Null)
            {
                return;
            }

            var eventName = GetEventKey<TEvent>();
            _handlers[eventName].Remove(subscriptionInformation);
            if (!_handlers[eventName].Any())
            {
                _handlers.Remove(eventName);

                var eventType = _eventTypes
                    .Single(e => e.Name == eventName);
                _eventTypes.Remove(eventType);

                RaiseOnEventRemoved(eventName);
                
                _logger.LogInformationIfEnabled(
                    "Subscription removed: {EventHandler} handler unsubscribed from {EventType} event",
                    subscriptionInformation.HandlerType.Name,
                    eventName);
            }
        }

        public void Clear() => _handlers.Clear();

        public string GetEventKey<T>() => typeof(T).Name;

        public Type GetEventTypeByName(string eventName)
            => _eventTypes.Single(t => t.Name == eventName);

        public IReadOnlyList<EventSubscriptionInformation> GetHandlersForEvent<T>() 
            where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return GetHandlersForEvent(key);
        }

        public IReadOnlyList<EventSubscriptionInformation> GetHandlersForEvent(string eventName)
            => _handlers[eventName];

        public bool HasSubscriptionsForEvent<T>() 
            where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return HasSubscriptionsForEvent(key);
        }

        public bool HasSubscriptionsForEvent(string eventName)
            => _handlers.ContainsKey(eventName);

        private EventSubscriptionInformation FindSubscriptionToRemove<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IEventHandler<TEvent>
        {
            var eventName = GetEventKey<TEvent>();

            if (!HasSubscriptionsForEvent(eventName))
            {
                return EventSubscriptionInformation.Null;
            }

            return _handlers[eventName]
                .Single(s => s.HandlerType == typeof(TEvent));
        }

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventName);
        }
    }
}
