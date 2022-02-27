namespace BudgetCast.Common.Messaging.Abstractions.Common
{
    public interface IMessagePreHandlingStep
    {
        Task Execute(IntegrationMessage message, CancellationToken cancellationToken);
    }
}
