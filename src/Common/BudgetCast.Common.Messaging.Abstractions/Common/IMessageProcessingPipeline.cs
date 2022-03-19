namespace BudgetCast.Common.Messaging.Abstractions.Common;

/// <summary>
/// Represents an abstraction over message processing pipeline
/// </summary>
public interface IMessageProcessingPipeline
{
    /// <summary>
    /// Handles received messages
    /// </summary>
    /// <param name="messageId">Message Id</param>
    /// <param name="messageName">Message Name</param>
    /// <param name="messageData">Message data (text based, such as JSON or XML)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<bool> Handle(string messageId, string messageName, string messageData, CancellationToken cancellationToken);
}