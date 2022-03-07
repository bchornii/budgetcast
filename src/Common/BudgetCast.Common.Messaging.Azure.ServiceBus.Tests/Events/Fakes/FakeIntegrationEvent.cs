using System;
using BudgetCast.Common.Messaging.Abstractions.Events;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events.Fakes;

internal class FakeIntegrationEvent : IntegrationEvent
{
    public FakeIntegrationEventState State { get; }

    public FakeIntegrationEvent()
    {
        State = new FakeIntegrationEventState();
    }

    public FakeIntegrationEvent(Guid id) : base(id, DateTime.Now)
    {
        State = new FakeIntegrationEventState();
    }
}