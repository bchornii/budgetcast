using BudgetCast.Common.Messaging.Abstractions.Events;

namespace BudgetCast.Notifications.AppHub.EventHandlers;

public class TestIntegrationEvent : IntegrationEvent
{
    public TestIntegrationEvent(Guid id) : base(id, DateTime.UtcNow)
    {
    }
}