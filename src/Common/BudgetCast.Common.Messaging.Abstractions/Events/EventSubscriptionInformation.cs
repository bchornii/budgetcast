namespace BudgetCast.Common.Messaging.Abstractions.Events
{
    public class EventSubscriptionInformation
    {
        public static readonly EventSubscriptionInformation Null = new(handlerType: null!);

        public Type HandlerType { get; }

        public EventSubscriptionInformation(Type handlerType)
        {
            HandlerType = handlerType;
        }
    }
}
