using BudgetCast.Common.Messaging.Abstractions.Messages;

namespace BudgetCast.Common.Messaging.Abstractions.Handlers
{
    public interface IIntegrationMessagePostHandlerStep
    {
        Task Execute<TMessage>(TMessage message, CancellationToken cancellationToken)
            where TMessage : IntegrationMessage;
    }
}
