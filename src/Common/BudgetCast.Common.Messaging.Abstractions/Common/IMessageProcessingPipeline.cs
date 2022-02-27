namespace BudgetCast.Common.Messaging.Abstractions.Common
{
    public interface IMessageProcessingPipeline
    {
        Task<bool> Handle(string eventName, string message, CancellationToken cancellationToken);
    }
}
