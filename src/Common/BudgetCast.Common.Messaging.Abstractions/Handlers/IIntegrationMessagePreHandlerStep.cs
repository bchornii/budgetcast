using BudgetCast.Common.Messaging.Abstractions.Messages;

namespace BudgetCast.Common.Messaging.Abstractions.Handlers
{
    public interface IIntegrationMessagePreHandlerStep
    {
        Task Execute(IntegrationMessage message, CancellationToken cancellationToken)
    }
}
