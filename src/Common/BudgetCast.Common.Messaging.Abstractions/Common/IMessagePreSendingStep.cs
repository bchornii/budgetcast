namespace BudgetCast.Common.Messaging.Abstractions.Common;

/// <summary>
/// Represents an abstraction over message pre-sending step which can optionally be executed prior
/// to message being sent, for example, to supplement a message an additional contextual information such
/// as user id or any other.
/// </summary>
public interface IMessagePreSendingStep
{
    /// <summary>
    /// Executes message pre-sending logic
    /// </summary>
    /// <param name="message">Integration message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task Execute(IntegrationMessage message, CancellationToken cancellationToken);
}