using Azure.Messaging.ServiceBus;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus
{
    public class ServiceBusPluginProcessor : ServiceBusProcessor
    {
        private readonly IEnumerable<Func<ServiceBusReceivedMessage, Task>> _plugins;

        internal ServiceBusPluginProcessor(
            string queueName, 
            ServiceBusClient client, 
            IEnumerable<Func<ServiceBusReceivedMessage, Task>> plugins, 
            ServiceBusProcessorOptions options) 
            : base(client, queueName, options)
        {
            _plugins = plugins;
        }

        internal ServiceBusPluginProcessor(
            string topicName, 
            string subscriptionName, 
            ServiceBusClient client, 
            IEnumerable<Func<ServiceBusReceivedMessage, Task>> plugins, 
            ServiceBusProcessorOptions options) 
            : base(client, topicName, subscriptionName, options)
        {
            _plugins = plugins;
        }

        protected override async Task OnProcessMessageAsync(ProcessMessageEventArgs args)
        {
            foreach (var plugin in _plugins)
            {
                await plugin.Invoke(args.Message);
            }

            await base.OnProcessMessageAsync(args);
        }
    }
}
