namespace BudgetCast.Common.Messaging.Abstractions.Events
{
    public class EventSubscriptionInformation
    {
        public static readonly EventSubscriptionInformation Null = new(eventType: null!, eventHandlerType: null!);

        public Type EventHandlerType { get; }
        
        public Type EventType { get; }

        public EventSubscriptionInformation(Type eventHandlerType, Type eventType)
        {
            EventHandlerType = eventHandlerType;
            EventType = eventType;
        }

        public override string ToString() 
            => $"[{EventType.Name}-{EventHandlerType.Name}]";
    }
}
