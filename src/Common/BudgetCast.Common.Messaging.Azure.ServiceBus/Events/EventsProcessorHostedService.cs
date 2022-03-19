using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Events;

/// <summary>
/// Hosted service used to start & stop event processing.
/// </summary>
public class EventsProcessorHostedService : IHostedService
{
    private readonly IEventsProcessor _eventsProcessor;

    public EventsProcessorHostedService(IEventsProcessor eventsProcessor) 
        => _eventsProcessor = eventsProcessor;

    /// <summary>
    /// Signals <see cref="IEventsProcessor"/> that processing shall start.
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task StartAsync(CancellationToken cancellationToken) 
        => await _eventsProcessor.Start(cancellationToken);

    /// <summary>
    /// Signals <see cref="IEventsProcessor"/> that processing shall be stopped.
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task StopAsync(CancellationToken cancellationToken) 
        => await _eventsProcessor.Stop(cancellationToken);
}