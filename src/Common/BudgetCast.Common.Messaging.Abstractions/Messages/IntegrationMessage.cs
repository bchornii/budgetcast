using System.Text.Json.Serialization;

namespace BudgetCast.Common.Messaging.Abstractions.Messages
{
    public abstract class IntegrationMessage
    {
        [JsonInclude]
        public Guid Id { get; protected init; }

        [JsonInclude]
        public DateTime CreatedAt { get; protected init; }
    }
}
