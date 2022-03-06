using System.Text.Json.Serialization;
using BudgetCast.Common.Messaging.Abstractions.Common;

namespace BudgetCast.Common.Messaging.Abstractions.Events
{
    public abstract class IntegrationEvent : IntegrationMessage
    {
        protected IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        [JsonConstructor]
        protected IntegrationEvent(Guid id, DateTime createdAt)
        {
            Id = id;
            CreatedAt = createdAt;
        }
    }
}
