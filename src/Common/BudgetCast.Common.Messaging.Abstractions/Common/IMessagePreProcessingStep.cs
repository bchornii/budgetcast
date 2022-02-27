namespace BudgetCast.Common.Messaging.Abstractions.Common
{
    public interface IMessagePreProcessingStep
    {
        Task Execute(IntegrationMessage message, CancellationToken cancellationToken);
    }
}
