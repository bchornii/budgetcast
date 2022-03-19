using Azure.Messaging.ServiceBus;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events.Fakes;

internal class FakeServiceBusClient : ServiceBusClient
{
    private FakeServiceBusSender Sender { get; }

    private FakeServiceBusProcessor Processor { get; }

    public FakeServiceBusClient(FakeServiceBusSender sender)
    {
        Sender = sender;
        Processor = default!;
    }

    public FakeServiceBusClient(FakeServiceBusProcessor processor)
    {
        Processor = processor;
        Sender = default!;
    }
            
    public override ServiceBusSender CreateSender(string queueOrTopicName) 
        => Sender;

    public override ServiceBusProcessor CreateProcessor(
        string topicName,
        string subscriptionName,
        ServiceBusProcessorOptions options) 
        => Processor;
}