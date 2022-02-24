using BudgetCast.Common.Messaging.Abstractions.Handlers;
using BudgetCast.Common.Messaging.Abstractions.Messages.Events;

namespace BudgetCast.Common.Messaging.AzServiceBus.Messages.Events
{
    public class EventSubscriptionManager : IEventSubscriptionManager
    {
        private readonly Dictionary<string, List<SubscriptionInformation>> _handlers;
        private readonly List<Type> _eventTypes;

        public bool HasNoSubscriptions => _handlers is { Count: 0 };

        public event EventHandler<string> OnEventRemoved;

        public EventSubscriptionManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInformation>>();
            _eventTypes = new List<Type>();

            OnEventRemoved = (_, __) => { };
        }

        public void AddSubscription<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = GetEventKey<TEvent>();

            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<SubscriptionInformation>());
            }

            var handlerType = typeof(THandler);
            if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered " +
                    $"for '{eventName}'", nameof(handlerType));
            }

            _handlers[eventName]
                .Add(new SubscriptionInformation(handlerType));

            if (!_eventTypes.Contains(typeof(TEvent)))
            {
                _eventTypes.Add(typeof(TEvent));
            }
        }

        public void RemoveSubscription<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            var subscriptionInformation = FindSubscriptionToRemove<TEvent, THandler>();

            if (subscriptionInformation == SubscriptionInformation.Null)
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
            }
        }

        public void Clear() => _handlers.Clear();

        public string GetEventKey<T>() => typeof(T).Name;

        public Type GetEventTypeByName(string eventName)
            => _eventTypes.Single(t => t.Name == eventName);

        public IReadOnlyList<SubscriptionInformation> GetHandlersForEvent<T>() 
            where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return GetHandlersForEvent(key);
        }

        public IReadOnlyList<SubscriptionInformation> GetHandlersForEvent(string eventName)
            => _handlers[eventName];

        public bool HasSubscriptionsForEvent<T>() 
            where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return HasSubscriptionsForEvent(key);
        }

        public bool HasSubscriptionsForEvent(string eventName)
            => _handlers.ContainsKey(eventName);

        private SubscriptionInformation FindSubscriptionToRemove<TEvent, THandler>()
            where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = GetEventKey<TEvent>();

            if (!HasSubscriptionsForEvent(eventName))
            {
                return SubscriptionInformation.Null;
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
