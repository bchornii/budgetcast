using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Events
{
    public class EventsProcessorHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventsProcessorHostedService> _logger;

        public EventsProcessorHostedService(IServiceProvider serviceProvider, ILogger<EventsProcessorHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var eventBus = _serviceProvider.GetRequiredService<IEventsProcessor>();
            await eventBus.Start(cancellationToken);

            _logger.LogInformationIfEnabled(
                "Started event processing at {StartedAt}", DateTime.UtcNow);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var eventBus = _serviceProvider.GetRequiredService<IEventsProcessor>();
            await eventBus.Stop(cancellationToken);
            
            _logger.LogInformationIfEnabled(
                "Stopped event processing at {StartedAt}", DateTime.UtcNow);
        }
    }
}
