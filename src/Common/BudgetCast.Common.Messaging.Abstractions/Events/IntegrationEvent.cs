using System.Text.Json.Serialization;
using BudgetCast.Common.Messaging.Abstractions.Common;

namespace BudgetCast.Common.Messaging.Abstractions.Events;

/// <summary>
/// Represents an abstraction over integration event. Inherits
/// from <see cref="IntegrationMessage"/>.
/// <remarks>
/// If default constructor is used to instantiate derived type, then an event id
/// will be initialized by randomly generated guid and event creation timestamp
/// with value of current UTC datetime value.
/// </remarks>
/// </summary>
public abstract class IntegrationEvent : IntegrationMessage
{
    protected IntegrationEvent()
    {
        Id = Guid.NewGuid().ToString("D");
        CreatedAt = DateTime.UtcNow;
    }

    [JsonConstructor]
    protected IntegrationEvent(string id, DateTime createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
    }
}