using Azure.Messaging.ServiceBus;

namespace BudgetCast.Common.Messaging.AzServiceBus;

public class PluginSender : ServiceBusSender
{
    private readonly IEnumerable<Func<ServiceBusMessage, Task>> _plugins;

    internal PluginSender(
        string queueOrTopicName,
        ServiceBusClient client,
        IEnumerable<Func<ServiceBusMessage, Task>> plugins)
        : base(client, queueOrTopicName)
    {
        _plugins = plugins;
    }

    public override async Task SendMessageAsync(ServiceBusMessage message, CancellationToken cancellationToken = default)
    {
        foreach (var plugin in _plugins)
        {
            await plugin.Invoke(message);
        }

        await base
            .SendMessageAsync(message, cancellationToken)
            .ConfigureAwait(false);
    }
}