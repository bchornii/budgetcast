using BudgetCast.Common.Messaging.Abstractions.Common;

namespace BudgetCast.Common.Messaging.Abstractions.Commands
{
    public interface ICommandBus : IMessagingBus
    {
        Task Send(IntegrationCommand command);

        void Bind<TCommand, THandler>()
            where TCommand : IntegrationCommand
            where THandler : ICommandHandler<TCommand>;
    }
}
