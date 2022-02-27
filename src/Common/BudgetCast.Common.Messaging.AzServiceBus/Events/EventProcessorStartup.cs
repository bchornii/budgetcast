using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.AzServiceBus.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Messaging.AzServiceBus.Events
{
    public class EventProcessorStartup : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventProcessorStartup> _logger;

        public EventProcessorStartup(IServiceProvider serviceProvider, ILogger<EventProcessorStartup> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var eventBus = _serviceProvider.GetRequiredService<IEventProcessor>();
            await eventBus.Start(cancellationToken);

            _logger.LogInformationIfEnabled(
                "Started event processing at {StartedAt}", DateTime.UtcNow);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var eventBus = _serviceProvider.GetRequiredService<IEventProcessor>();
            await eventBus.Stop(cancellationToken);
            
            _logger.LogInformationIfEnabled(
                "Stopped event processing at {StartedAt}", DateTime.UtcNow);
        }
    }
}
