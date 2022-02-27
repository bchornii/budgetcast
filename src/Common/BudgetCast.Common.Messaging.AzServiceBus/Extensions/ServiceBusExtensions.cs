using Azure.Messaging.ServiceBus;

namespace BudgetCast.Common.Messaging.AzServiceBus.Extensions;

public static class ServiceBusExtensions
{
    public static PluginSender CreatePluginSender(
        this ServiceBusClient client,
        string queueOrTopicName)
    {
        return new PluginSender(queueOrTopicName, client, Array.Empty<Func<ServiceBusMessage, Task>>());
    }

    public static PluginSender CreatePluginSender(
        this ServiceBusClient client,
        string queueOrTopicName,
        IEnumerable<Func<ServiceBusMessage, Task>> plugins)
    {
        return new PluginSender(queueOrTopicName, client, plugins);
    }

    public static PluginProcessor CreatePluginProcessor(
        this ServiceBusClient client,
        string queueName,
        ServiceBusProcessorOptions? options = default)
    {
        return new PluginProcessor(
            queueName, 
            client, 
            Array.Empty<Func<ServiceBusReceivedMessage, Task>>(), 
            options ?? new ServiceBusProcessorOptions());
    }

    public static PluginProcessor CreatePluginProcessor(
        this ServiceBusClient client,
        string queueName,
        IEnumerable<Func<ServiceBusReceivedMessage, Task>> plugins,
        ServiceBusProcessorOptions? options = default)
    {
        return new PluginProcessor(queueName, client, plugins, options ?? new ServiceBusProcessorOptions());
    }

    public static PluginProcessor CreatePluginProcessor(
        this ServiceBusClient client,
        string topicName,
        string subscriptionName,
        ServiceBusProcessorOptions? options = default)
    {
        return new PluginProcessor(
            topicName, 
            subscriptionName, 
            client, 
            Array.Empty<Func<ServiceBusReceivedMessage, Task>>(), 
            options ?? new ServiceBusProcessorOptions());
    }

    public static PluginProcessor CreatePluginProcessor(
        this ServiceBusClient client,
        string topicName,
        string subscriptionName,
        IEnumerable<Func<ServiceBusReceivedMessage, Task>> plugins,
        ServiceBusProcessorOptions? options = default)
    {
        return new PluginProcessor(
            topicName, 
            subscriptionName, 
            client, 
            plugins, 
            options ?? new ServiceBusProcessorOptions());
    }
}