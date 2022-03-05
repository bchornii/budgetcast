namespace BudgetCast.Common.Messaging.Abstractions.Common;

public interface IMessageSerializer
{
    /// <summary>
    /// Packs integration message as a JSON object.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    string PackAsJson(IntegrationMessage message);

    /// <summary>
    /// Unpacks integration message from JSON.
    /// </summary>
    /// <param name="messageData">Message data in JSON format</param>
    /// <param name="messageType">Message type which should be instantiated</param>
    /// <returns></returns>
    object? UnpackFromJson(string messageData, Type messageType);
}