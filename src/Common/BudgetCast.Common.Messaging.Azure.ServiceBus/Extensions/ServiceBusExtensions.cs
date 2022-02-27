using Azure.Messaging.ServiceBus;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;

public static class ServiceBusExtensions
{
    public static ServiceBusPluginSender CreatePluginSender(
        this ServiceBusClient client,
        string queueOrTopicName)
    {
        return new ServiceBusPluginSender(queueOrTopicName, client, Array.Empty<Func<ServiceBusMessage, Task>>());
    }

    public static ServiceBusPluginSender CreatePluginSender(
        this ServiceBusClient client,
        string queueOrTopicName,
        IEnumerable<Func<ServiceBusMessage, Task>> plugins)
    {
        return new ServiceBusPluginSender(queueOrTopicName, client, plugins);
    }

    public static ServiceBusPluginProcessor CreatePluginProcessor(
        this ServiceBusClient client,
        string queueName,
        ServiceBusProcessorOptions? options = default)
    {
        return new ServiceBusPluginProcessor(
            queueName, 
            client, 
            Array.Empty<Func<ServiceBusReceivedMessage, Task>>(), 
            options ?? new ServiceBusProcessorOptions());
    }

    public static ServiceBusPluginProcessor CreatePluginProcessor(
        this ServiceBusClient client,
        string queueName,
        IEnumerable<Func<ServiceBusReceivedMessage, Task>> plugins,
        ServiceBusProcessorOptions? options = default)
    {
        return new ServiceBusPluginProcessor(queueName, client, plugins, options ?? new ServiceBusProcessorOptions());
    }

    public static ServiceBusPluginProcessor CreatePluginProcessor(
        this ServiceBusClient client,
        string topicName,
        string subscriptionName,
        ServiceBusProcessorOptions? options = default)
    {
        return new ServiceBusPluginProcessor(
            topicName, 
            subscriptionName, 
            client, 
            Array.Empty<Func<ServiceBusReceivedMessage, Task>>(), 
            options ?? new ServiceBusProcessorOptions());
    }

    public static ServiceBusPluginProcessor CreatePluginProcessor(
        this ServiceBusClient client,
        string topicName,
        string subscriptionName,
        IEnumerable<Func<ServiceBusReceivedMessage, Task>> plugins,
        ServiceBusProcessorOptions? options = default)
    {
        return new ServiceBusPluginProcessor(
            topicName, 
            subscriptionName, 
            client, 
            plugins, 
            options ?? new ServiceBusProcessorOptions());
    }
}