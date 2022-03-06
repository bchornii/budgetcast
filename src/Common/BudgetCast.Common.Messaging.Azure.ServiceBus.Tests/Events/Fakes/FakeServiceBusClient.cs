using Azure.Messaging.ServiceBus;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events.Fakes;

internal class FakeServiceBusClient : ServiceBusClient
{
    public FakeServiceBusSender Sender { get; }

    public FakeServiceBusClient(FakeServiceBusSender sender)
    {
        Sender = sender;
    }
            
    public override ServiceBusSender CreateSender(string queueOrTopicName) 
        => Sender;
}