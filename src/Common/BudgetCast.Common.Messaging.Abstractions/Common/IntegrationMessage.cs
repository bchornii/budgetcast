using System.Text.Json.Serialization;

namespace BudgetCast.Common.Messaging.Abstractions.Common
{
    public abstract class IntegrationMessage
    {
        public const string UserIdMetadataKey = "UserId";
        public const string TenantIdMetadataKey = "TenantId";

        [JsonInclude]
        public Guid Id { get; protected init; }

        [JsonInclude]
        public DateTime CreatedAt { get; protected init; }

        [JsonInclude]
        public IDictionary<string, string> Metadata { get; private set; }

        protected IntegrationMessage()
        {
            Metadata = new Dictionary<string, string>();
        }

        /// <summary>
        /// Retrieves user id from message metadata.
        /// </summary>
        /// <returns></returns>
        public string GetUserId() => Metadata
            .TryGetValue(UserIdMetadataKey, out var value) 
            ? value 
            : string.Empty;

        /// <summary>
        /// Retrieves tenant id from message metadata.
        /// </summary>
        /// <returns></returns>
        public long? GetTenantId() => Metadata
            .TryGetValue(TenantIdMetadataKey, out var value) 
            ? long.Parse(value) 
            : null;

        /// <summary>
        /// Sets tenant identifier.
        /// </summary>
        /// <param name="tenantId"></param>
        public void SetCurrentTenant(long tenantId) => 
            Metadata[nameof(TenantIdMetadataKey)] = tenantId.ToString();

        /// <summary>
        /// Sets user identifier.
        /// </summary>
        /// <param name="userId"></param>
        public void SetUserId(string userId) 
            => Metadata[nameof(UserIdMetadataKey)] = userId;
    }
}
