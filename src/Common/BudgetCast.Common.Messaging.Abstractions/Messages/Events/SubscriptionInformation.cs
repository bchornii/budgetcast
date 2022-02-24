namespace BudgetCast.Common.Messaging.Abstractions.Messages.Events
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
