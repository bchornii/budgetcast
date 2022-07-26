using BudgetCast.Common.Domain;
using MediatR;

namespace BudgetCast.Common.Data.DomainEvents;

public class DomainEventsDispatcher : IDomainEventsDispatcher
{
    private readonly IMediator _mediator;
    private readonly OperationalDbContext _dbContext;

    public DomainEventsDispatcher(IMediator mediator, OperationalDbContext dbContext)
    {
        _mediator = mediator;
        _dbContext = dbContext;
    }
    
    // </inherits> 
    public async Task DispatchEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = _dbContext.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents?.Any() ?? false)
            .ToList();

        var domainEventType = typeof(IDomainEvent);
        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents!
                .Where(de => de.GetType().IsAssignableTo(domainEventType)))
            .ToList();
        
        domainEntities.ForEach(entry => entry.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }

    // </inherits> 
    public async Task DispatchNotificationsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = _dbContext.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEventNotifications?.Any() ?? false)
            .ToList();

        var domainEventNotificationType = typeof(IDomainEventNotification);
        var domainEventsNotifications = domainEntities
            .SelectMany(x => x.Entity.DomainEventNotifications!
                .Where(de => de.GetType().IsAssignableTo(domainEventNotificationType)))
            .ToList();
        
        domainEntities.ForEach(entry => entry.Entity.ClearDomainEventNotifications());

        foreach (var domainEventNotification in domainEventsNotifications)
        {
            await _mediator.Publish(domainEventNotification, cancellationToken);
        }
    }
}