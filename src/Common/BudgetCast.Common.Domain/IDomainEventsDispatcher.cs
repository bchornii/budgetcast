namespace BudgetCast.Common.Domain;

public interface IDomainEventsDispatcher
{
    /// <summary>
    /// Dispatching domain events.
    /// </summary>
    /// <returns></returns>
    Task DispatchEventsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Dispatching domain event notifications.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DispatchNotificationsAsync(CancellationToken cancellationToken);
}