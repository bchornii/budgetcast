namespace BudgetCast.Common.Messaging.Abstractions.Handlers
{
    public interface IIntegrationMessageProcessingPipeline
    {
        Task<bool> Process(string eventName, string message, CancellationToken cancellationToken);
    }
}
