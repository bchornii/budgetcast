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
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;

    public UnitOfWork(
        OperationalDbContext dbContext,
        ILogger<UnitOfWork> logger, 
        IIntegrationEventLogService eventLogService, 
        IEventsPublisher eventsPublisher, 
        IDomainEventsDispatcher domainEventsDispatcher)
    {
        _dbContext = dbContext;
        _logger = logger;
        _eventLogService = eventLogService;
        _eventsPublisher = eventsPublisher;
        _domainEventsDispatcher = domainEventsDispatcher;
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
            await using var transaction = await dbContext.BeginTransactionAsync(token);
            _logger.LogInformation("----- Begin transaction {TransactionId}", transaction.TransactionId);
            
            _logger.LogInformation("Dispatching domain events for {TransactionId} transaction", transaction.TransactionId);
            await _domainEventsDispatcher.DispatchEventsAsync(token);
            
            _logger.LogInformation("Saving changes to the database transaction {TransactionId})", transaction.TransactionId);
            var result = await dbContext.SaveChangesAsync(token);
            
            await dbContext.CommitTransactionAsync(transaction, token);
            _logger.LogInformation("----- Commit transaction {TransactionId}", transaction.TransactionId);
            
            return result;
        }, cancellationToken);

        await _domainEventsDispatcher.DispatchNotificationsAsync(cancellationToken);
        
        await PublishPendingIntegrationEvents(cancellationToken);

        return result > 0;
    }
        
    // TODO: Move this logic into separate MediatR behavior which wraps Idempotent Behavior to handle the case
    // TODO: when CommandHandler was processed, business data and outgoing message are stored in database but processing failed
    // TODO: on '_eventsPublisher.Publish' execution (no exception handling). In such a case client might retry operation
    // TODO: but since there is a record in idempotent tracking store, command handler won't be triggered again, and UoW won't resent message.
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
                // TODO: reevaluate if that's appropriate approach for web and event handling processing scenarios. For event-based
                _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId}", logEvt.EventId);
                await _eventLogService.MarkEventAsFailedAsync(logEvt.EventId);
            }
        }
    }
}