namespace BudgetCast.Common.Messaging.Abstractions.Common;

/// <summary>
/// Represents an abstraction over message post-processing step which can optionally be
/// executed after message handling.
/// </summary>
public interface IMessagePostProcessingStep
{
    /// <summary>
    /// Executes message post processing logic
    /// </summary>
    /// <param name="message">Integration message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task Execute(IntegrationMessage message, CancellationToken cancellationToken);
}