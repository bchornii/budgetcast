namespace BudgetCast.Common.Messaging.Abstractions.Common;

/// <summary>
/// Represents an abstraction over message pre-processing step which can optionally be executed prior
/// to message handling, for example, to supplement a message an additional contextual information such
/// as user id or any other.
/// </summary>
public interface IMessagePreProcessingStep
{
    /// <summary>
    /// Executes message pre-processing logic
    /// </summary>
    /// <param name="message">Integration message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task Execute(IntegrationMessage message, CancellationToken cancellationToken);
}