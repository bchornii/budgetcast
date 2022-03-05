namespace BudgetCast.Common.Messaging.Abstractions.Common
{
    /// <summary>
    /// Defines general pipeline for message processing
    /// </summary>
    public interface IMessageProcessingPipeline
    {
        /// <summary>
        /// Handles message received.
        /// </summary>
        /// <param name="messageId">Message Id</param>
        /// <param name="messageName">Message Name</param>
        /// <param name="messageData">Message data (text based, such as JSON or XML)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<bool> Handle(string messageId, string messageName, string messageData, CancellationToken cancellationToken);
    }
}
