using BudgetCast.Common.Application.Outbox;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Expenses.Commands;
using Microsoft.EntityFrameworkCore;

namespace BudgetCast.Common.Data.EventLog;

public class IntegrationEventLogService : IIntegrationEventLogService
{
    private readonly OperationalDbContext _dbContext;
    private readonly List<Type> _eventTypes;

    private string ScopeId { get; }

    public IntegrationEventLogService(OperationalDbContext dbContext, Func<List<Type>> eventTypes)
    {
        _dbContext = dbContext;
        _eventTypes = eventTypes();

        ScopeId = Guid.NewGuid().ToString("N");
    }
    
    /// <summary>
    /// Retrieve all events pending to be published in a scope of current operation set.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveScopedEventsPendingToPublishAsync()
    {
        var result = await _dbContext.IntegrationEventLogs
            .Where(e => e.ScopeId == ScopeId && e.State == EventStateEnum.NotPublished)
            .ToListAsync();

        if (result.Any())
        {
            return result
                .OrderBy(o => o.CreationTime)
                .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)!));
        }

        return Array.Empty<IntegrationEventLogEntry>();
    }
    
    /// <summary>
    /// Retrieve events pending to be published which were emitted in scope of particular transaction.
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventsPendingToPublishAsync(Guid transactionId)
    {
        var tid = transactionId.ToString();

        var result = await _dbContext.IntegrationEventLogs
            .Where(e => e.TransactionId == tid && e.State == EventStateEnum.NotPublished)
            .ToListAsync();

        if (result.Any())
        {
            return result
                .OrderBy(o => o.CreationTime)
                .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)!));
        }

        return Array.Empty<IntegrationEventLogEntry>();
    }

    /// <summary>
    /// Add event to event store. Note, at this point it won't be actually saved.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task AddEventAsync(IntegrationEvent @event, CancellationToken cancellationToken)
    {
        var eventLogEntry = new IntegrationEventLogEntry(@event, ScopeId);
        _dbContext.IntegrationEventLogs.Add(eventLogEntry);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Marks events as published.
    /// </summary>
    /// <param name="eventId">Integration event id</param>
    public async Task MarkEventAsPublishedAsync(string eventId) 
        => await UpdateEventStatus(eventId, EventStateEnum.Published);

    /// <summary>
    /// Marks event as in progress.
    /// </summary>
    /// <param name="eventId">Integration event id</param>
    public async Task MarkEventAsInProgressAsync(string eventId) 
        => await UpdateEventStatus(eventId, EventStateEnum.InProgress);

    /// <summary>
    /// Marks event as failed to be published
    /// </summary>
    /// <param name="eventId">Integration event id</param>
    public async Task MarkEventAsFailedAsync(string eventId) 
        => await UpdateEventStatus(eventId, EventStateEnum.PublishedFailed);

    private async Task UpdateEventStatus(string eventId, EventStateEnum status)
    {
        var eventLogEntry = await _dbContext.IntegrationEventLogs
            .SingleAsync(ie => ie.EventId == eventId);
        eventLogEntry.State = status;

        if (status == EventStateEnum.InProgress)
        {
            eventLogEntry.TimesSent++;
        }

        await _dbContext.SaveChangesAsync();
    }
}