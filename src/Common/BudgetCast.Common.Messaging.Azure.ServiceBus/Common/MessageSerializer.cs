using System.Text.Json;
using BudgetCast.Common.Messaging.Abstractions.Common;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Common;

public class MessageSerializer : IMessageSerializer
{
    public string PackAsJson(IntegrationMessage message) 
        => JsonSerializer.Serialize(message, message.GetType());

    public object? UnpackFromJson(string message, Type messageType) 
        => JsonSerializer.Deserialize(message, messageType);
}