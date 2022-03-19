using BudgetCast.Common.Messaging.Abstractions.Common;

namespace BudgetCast.Common.Messaging.Abstractions.Events;

/// <summary>
/// Represents an abstraction over event handler
/// </summary>
/// <typeparam name="TEvent">Integration event type</typeparam>
public interface IEventHandler<in TEvent> : IMessageHandler
    where TEvent : IntegrationEvent
{
    /// <summary>
    /// Handles received event
    /// </summary>
    /// <param name="event">Integration event</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task Handle(TEvent @event, CancellationToken cancellationToken);
}