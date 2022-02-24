using System.Text.Json.Serialization;

namespace BudgetCast.Common.Messaging.Abstractions.Messages.Events
{
    public class IntegrationEvent : IntegrationMessage
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime createdAt)
        {
            Id = id;
            CreatedAt = createdAt;
        }
    }
}
