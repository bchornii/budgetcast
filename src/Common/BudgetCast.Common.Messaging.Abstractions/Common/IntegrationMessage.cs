using System.Text.Json.Serialization;

namespace BudgetCast.Common.Messaging.Abstractions.Common;

/// <summary>
/// Represents an abstraction over any integration message or event
/// </summary>
public abstract class IntegrationMessage
{
    public const string UserIdMetadataKey = "UserId";
    public const string TenantIdMetadataKey = "TenantId";

    /// <summary>
    /// Message or event id
    /// </summary>
    [JsonInclude]
    public string Id { get; protected init; }

    /// <summary>
    /// Message creation timestamp
    /// </summary>
    [JsonInclude]
    public DateTime CreatedAt { get; protected init; }

    /// <summary>
    /// Optional contextual metadata
    /// </summary>
    [JsonInclude]
    public IDictionary<string, string> Metadata { get; private set; }

    protected IntegrationMessage()
    {
        Id = default!;
        Metadata = new Dictionary<string, string>();
    }

    /// <summary>
    /// Retrieves user id from message metadata.
    /// </summary>
    /// <returns></returns>
    public virtual string GetUserId() => Metadata
        .TryGetValue(UserIdMetadataKey, out var value) 
        ? value 
        : string.Empty;

    /// <summary>
    /// Retrieves tenant id from message metadata.
    /// </summary>
    /// <returns></returns>
    public virtual long? GetTenantId() => Metadata
        .TryGetValue(TenantIdMetadataKey, out var value) 
        ? long.Parse(value) 
        : null;

    /// <summary>
    /// Sets tenant identifier.
    /// </summary>
    /// <param name="tenantId"></param>
    public virtual void SetCurrentTenant(long tenantId) => 
        Metadata[TenantIdMetadataKey] = tenantId.ToString();

    /// <summary>
    /// Sets user identifier.
    /// </summary>
    /// <param name="userId"></param>
    public virtual void SetUserId(string userId) 
        => Metadata[UserIdMetadataKey] = userId;
}