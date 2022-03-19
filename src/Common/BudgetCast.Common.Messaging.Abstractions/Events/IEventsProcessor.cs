using BudgetCast.Common.Messaging.Abstractions.Common;

namespace BudgetCast.Common.Messaging.Abstractions.Events;

/// <summary>
/// Represents an abstraction over long running events processor
/// </summary>
public interface IEventsProcessor
{
    /// <summary>
    /// Starts events processing. Should be called once in a lifecycle,
    /// usually on an application start
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task Start(CancellationToken cancellationToken);

    /// <summary>
    /// Stops event processing. Should be called when no further events processing
    /// is expected to happen. Most often is called on an application stop
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task Stop(CancellationToken cancellationToken);
        
    /// <summary>
    /// Subscribes an event to an event handler
    /// </summary>
    /// <typeparam name="TEvent">Integration event type</typeparam>
    /// <typeparam name="THandler">Integration event handler type</typeparam>
    /// <returns></returns>
    Task SubscribeTo<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IEventHandler<TEvent>;

    /// <summary>
    /// Unsubscribes an event handler from an event
    /// </summary>
    /// <typeparam name="TEvent">Integration event type</typeparam>
    /// <typeparam name="THandler">Integration event handler type</typeparam>
    /// <returns></returns>
    Task UnsubscribeFrom<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IEventHandler<TEvent>;
}