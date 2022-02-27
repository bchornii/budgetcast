namespace BudgetCast.Common.Messaging.Abstractions.Common
{
    public interface IMessagePostProcessingStep
    {
        Task Execute(IntegrationMessage message, CancellationToken cancellationToken);
    }
}
