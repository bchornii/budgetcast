using BudgetCast.Common.Messaging.Abstractions.Messages.Commands;

namespace BudgetCast.Common.Messaging.Abstractions.Handlers
{
    public interface IIntegrationCommandHandler<in TCommand> : IIntegrationMessageHandler
        where TCommand : IntegrationCommand
    {
        Task Handle(TCommand command);
    }
}
