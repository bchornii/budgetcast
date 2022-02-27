namespace BudgetCast.Common.Messaging.Abstractions.Common
{
    public interface IMessageHandlingPipeline
    {
        Task<bool> Handle(string eventName, string message, CancellationToken cancellationToken);
    }
}
