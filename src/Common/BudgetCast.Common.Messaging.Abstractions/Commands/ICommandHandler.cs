using BudgetCast.Common.Messaging.Abstractions.Common;

namespace BudgetCast.Common.Messaging.Abstractions.Commands
{
    public interface ICommandHandler<in TCommand> : IMessageHandler
        where TCommand : IntegrationCommand
    {
        Task Handle(TCommand command);
    }
}
