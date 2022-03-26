using BudgetCast.Common.Messaging.Abstractions.Common;

namespace BudgetCast.Common.Messaging.Abstractions.Events;

/// <summary>
/// Represents an abstraction over events publisher
/// </summary>
public interface IEventsPublisher : IMessageSender
{
    /// <summary>
    /// Publishes event to an underlying transport
    /// </summary>
    /// <param name="event">Integration event type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<bool> Publish(IntegrationEvent @event, CancellationToken cancellationToken);
}