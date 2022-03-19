using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Events;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Events;

/// <summary>
/// Implementation of in-memory events subscription manager which keeps track of
/// added/removed integration events subscriptions.
/// </summary>
public class EventsSubscriptionManager : IEventsSubscriptionManager
{
    private readonly ILogger<EventsSubscriptionManager> _logger;
    private readonly Dictionary<string, List<EventSubscriptionInformation>> _eventNameSubscriptionMap;
    private readonly List<Type> _eventTypes;

    public bool HasNoSubscriptions => _eventNameSubscriptionMap is { Count: 0 };

    public event EventHandler<string> OnEventRemoved = (_, __) => { };

    public EventsSubscriptionManager(ILogger<EventsSubscriptionManager> logger)
    {
        _logger = logger;
        _eventNameSubscriptionMap = new Dictionary<string, List<EventSubscriptionInformation>>();
        _eventTypes = new List<Type>();
    }

    public void AddSubscription<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        var eventName = GetEventKey<TEvent>();

        if (!HasSubscriptionsForEvent(eventName))
        {
            _eventNameSubscriptionMap.Add(eventName, new List<EventSubscriptionInformation>());
        }

        var eventHandlerType = typeof(THandler);
        var hasSubscriptionToTheSameHandler = _eventNameSubscriptionMap[eventName]
            .Any(s => s.EventHandlerType == eventHandlerType);
            
        if (hasSubscriptionToTheSameHandler)
        {
            throw new ArgumentException(
                $"Handler Type {eventHandlerType.Name} already registered " +
                $"for '{eventName}'", nameof(eventHandlerType));
        }

        var eventSubscription = new EventSubscriptionInformation(
            eventType: typeof(TEvent),
            eventHandlerType: eventHandlerType);
        
        _eventNameSubscriptionMap[eventName].Add(eventSubscription);

        if (!_eventTypes.Contains(typeof(TEvent)))
        {
            _eventTypes.Add(typeof(TEvent));
        }

        _logger.LogInformationIfEnabled("{Subscription} subscription has been created", eventSubscription);
    }

    public void RemoveSubscription<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        var eventSubscription = FindSubscriptionToRemove<TEvent, THandler>();

        if (eventSubscription == EventSubscriptionInformation.Null)
        {
            return;
        }

        var eventName = GetEventKey<TEvent>();
        _eventNameSubscriptionMap[eventName].Remove(eventSubscription);
            
        if (!_eventNameSubscriptionMap[eventName].Any())
        {
            _eventNameSubscriptionMap.Remove(eventName);

            var eventType = _eventTypes.Single(e => e.Name == eventName);
            _eventTypes.Remove(eventType);

            RaiseOnEventRemoved(eventName);

            _logger.LogInformationIfEnabled("{Subscription} subscription has been removed", eventSubscription);
        }
    }

    public void RemoveAll() 
        => _eventNameSubscriptionMap.Clear();
    
    public bool HasSubscriptionsForEvent<T>() 
        where T : IntegrationEvent
    {
        var key = GetEventKey<T>();
        return HasSubscriptionsForEvent(key);
    }

    public bool HasSubscriptionsForEvent(string eventName)
        => _eventNameSubscriptionMap.ContainsKey(eventName);
    
    public string GetEventKey<T>() 
        => typeof(T).Name;

    public Type GetEventTypeByName(string eventName)
        => _eventTypes.Single(t => t.Name == eventName);

    public IReadOnlyList<EventSubscriptionInformation> GetHandlersForEvent<T>() 
        where T : IntegrationEvent
    {
        var key = GetEventKey<T>();
        return GetHandlersForEvent(key);
    }

    public IReadOnlyList<EventSubscriptionInformation> GetHandlersForEvent(string eventName)
        => _eventNameSubscriptionMap[eventName];

    private EventSubscriptionInformation FindSubscriptionToRemove<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        var eventName = GetEventKey<TEvent>();

        if (!HasSubscriptionsForEvent(eventName))
        {
            return EventSubscriptionInformation.Null;
        }

        return _eventNameSubscriptionMap[eventName]
            .Single(s => s.EventHandlerType == typeof(THandler));
    }

    private void RaiseOnEventRemoved(string eventName)
    {
        var handler = OnEventRemoved;
        handler?.Invoke(this, eventName);
    }
}