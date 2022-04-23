using BudgetCast.Common.Application.Outbox;
using BudgetCast.Common.Messaging.Abstractions.Events;

namespace BudgetCast.Expenses.Commands;

public interface IIntegrationEventLogService
{
    /// <summary>
    /// Retrieve all events pending to be published.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<IntegrationEventLogEntry>> RetrieveScopedEventsPendingToPublishAsync();

    /// <summary>
    /// Add event to event store. Note, at this point it won't be actually saved.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddEventAsync(IntegrationEvent @event, CancellationToken cancellationToken);

    /// <summary>
    /// Marks events as published.
    /// </summary>
    /// <param name="eventId">Integration event id</param>
    Task MarkEventAsPublishedAsync(string eventId);

    /// <summary>
    /// Marks event as in progress.
    /// </summary>
    /// <param name="eventId">Integration event id</param>
    Task MarkEventAsInProgressAsync(string eventId);

    /// <summary>
    /// Marks event as failed to be published
    /// </summary>
    /// <param name="eventId">Integration event id</param>
    Task MarkEventAsFailedAsync(string eventId);
}