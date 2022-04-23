using BudgetCast.Common.Domain;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Expenses.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace BudgetCast.Common.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly OperationalDbContext _dbContext;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly IIntegrationEventLogService _eventLogService;
    private readonly IEventsPublisher _eventsPublisher;

    public UnitOfWork(
        OperationalDbContext dbContext,
        ILogger<UnitOfWork> logger, 
        IIntegrationEventLogService eventLogService, 
        IEventsPublisher eventsPublisher)
    {
        _dbContext = dbContext;
        _logger = logger;
        _eventLogService = eventLogService;
        _eventsPublisher = eventsPublisher;
    }
        
    public async Task<bool> Commit(CancellationToken cancellationToken)
    {
        if (_dbContext.HasActiveTransaction)
        {
            return true;
        }
        
        var strategy = _dbContext.Database.CreateExecutionStrategy();
        
        var result = await strategy.ExecuteAsync(_dbContext, async (dbContext, token) =>
        {
            await using var transaction = await dbContext.BeginTransactionAsync();
            _logger.LogInformation("----- Begin transaction {TransactionId}", transaction.TransactionId);
            
            _logger.LogInformation("Saving changes to the database transaction {TransactionId})", transaction.TransactionId);
            var result = await dbContext.SaveChangesAsync(token);
            
            await dbContext.CommitTransactionAsync(transaction, token);
            _logger.LogInformation("----- Commit transaction {TransactionId}", transaction.TransactionId);
            
            return result;
        }, cancellationToken);
        
        await PublishPendingIntegrationEvents(cancellationToken);

        return result > 0;
    }
        
    private async Task PublishPendingIntegrationEvents(CancellationToken cancellationToken)
    {
        var pendingLogEvents = await _eventLogService
            .RetrieveScopedEventsPendingToPublishAsync();

        foreach (var logEvt in pendingLogEvents)
        {
            try
            {
                await _eventLogService.MarkEventAsInProgressAsync(logEvt.EventId);
             
                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId}", logEvt.EventId);
                await _eventsPublisher.Publish(logEvt.IntegrationEvent, cancellationToken);
                _logger.LogInformation("----- Published integration event: {IntegrationEventId}", logEvt.EventId);
                
                await _eventLogService.MarkEventAsPublishedAsync(logEvt.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId}", logEvt.EventId);
                await _eventLogService.MarkEventAsFailedAsync(logEvt.EventId);
            }
        }
    }
}