using System.Text.Json.Serialization;

namespace BudgetCast.Common.Messaging.Abstractions.Common;

/// <summary>
/// Represents an abstraction over any integration message or event
/// </summary>
public abstract class IntegrationMessage
{
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
    /// Checks if metadata contains specific key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual bool HasMetadata(string key) 
        => Metadata.ContainsKey(key);

    public virtual string GetMetadata(string key)
        => Metadata.TryGetValue(key, out var value) ? value : string.Empty;
    
    /// <summary>
    /// Adds or updates (if exists) arbitrary metadata for a message.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public virtual void SetMetadata(string key, string value)
        => Metadata[key] = value;
}