namespace BudgetCast.Common.Messaging.Abstractions.Events;

/// <summary>
/// Represents event subscription
/// </summary>
public class EventSubscriptionInformation
{
    public static readonly EventSubscriptionInformation Null = 
        new(eventType: null!, eventHandlerType: null!);

    /// <summary>
    /// Event handler type
    /// </summary>
    public Type EventHandlerType { get; }
        
    /// <summary>
    /// Event type
    /// </summary>
    public Type EventType { get; }

    public EventSubscriptionInformation(Type eventHandlerType, Type eventType)
    {
        EventHandlerType = eventHandlerType;
        EventType = eventType;
    }

    public override string ToString() 
        => $"[{EventType.Name}-{EventHandlerType.Name}]";
}