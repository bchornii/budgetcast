using BudgetCast.Common.Messaging.Abstractions.Handlers;
using BudgetCast.Common.Messaging.Abstractions.Messages.Commands;

namespace BudgetCast.Common.Messaging.Abstractions.Communication
{
    public interface ICommandBus : IMessagingBus
    {
        Task Send(IntegrationCommand command);

        void Bind<TCommand, THandler>()
            where TCommand : IntegrationCommand
            where THandler : IIntegrationCommandHandler<TCommand>;
    }
}
