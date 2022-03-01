using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Events
{
    public class EventsProcessorHostedService : IHostedService
    {
        private readonly IEventsProcessor _eventsProcessor;

        public EventsProcessorHostedService(IEventsProcessor eventsProcessor)
        {
            _eventsProcessor = eventsProcessor;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _eventsProcessor.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _eventsProcessor.Stop(cancellationToken);
        }
    }
}
