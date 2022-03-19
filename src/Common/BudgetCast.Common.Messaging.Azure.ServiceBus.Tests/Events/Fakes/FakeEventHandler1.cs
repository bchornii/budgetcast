using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Common.Messaging.Abstractions.Events;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events.Fakes;

internal class FakeEventHandler1 : IEventHandler<FakeIntegrationEvent>
{
    public Task Handle(FakeIntegrationEvent @event, CancellationToken cancellationToken)
    {
        @event.State.IsProcessed = true;
        return Task.CompletedTask;
    }
}