namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events.Fakes;

internal class FakeIntegrationEventState
{
    public bool IsPreProcessed { get; set; }
        
    public bool IsProcessed { get; set; }
        
    public bool IsPostProcessed { get; set; }
}