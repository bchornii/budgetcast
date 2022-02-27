namespace BudgetCast.Common.Messaging.Abstractions.Events
{
    public class SubscriptionInformation
    {
        public static readonly SubscriptionInformation Null = new(handlerType: null!);

        public Type HandlerType { get; }

        public SubscriptionInformation(Type handlerType)
        {
            HandlerType = handlerType;
        }
    }
}
