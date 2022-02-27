namespace BudgetCast.Common.Messaging.Abstractions.Common
{
    public interface IMessagePostHandlingStep
    {
        Task Execute(IntegrationMessage message, CancellationToken cancellationToken);
    }
}
