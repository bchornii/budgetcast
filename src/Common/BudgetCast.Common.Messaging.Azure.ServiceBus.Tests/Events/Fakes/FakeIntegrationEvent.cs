using BudgetCast.Common.Messaging.Abstractions.Events;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events.Fakes;

internal class FakeIntegrationEvent : IntegrationEvent
{
    public FakeIntegrationEventState State { get; }

    public FakeIntegrationEvent()
    {
        State = new FakeIntegrationEventState();
    }
}