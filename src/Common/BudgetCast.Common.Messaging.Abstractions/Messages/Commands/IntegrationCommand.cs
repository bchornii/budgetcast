using System.Text.Json.Serialization;

namespace BudgetCast.Common.Messaging.Abstractions.Messages.Commands
{
    public class IntegrationCommand : IntegrationMessage
    {
        public IntegrationCommand()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        [JsonConstructor]
        public IntegrationCommand(Guid id, DateTime createdAt)
        {
            Id = id;
            CreatedAt = createdAt;
        }
    }
}
