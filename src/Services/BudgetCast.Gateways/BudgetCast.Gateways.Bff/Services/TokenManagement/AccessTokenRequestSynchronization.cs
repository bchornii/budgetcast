using System.Collections.Concurrent;

namespace BudgetCast.Gateways.Bff.Services.TokenManagement;

/// <summary>
/// Default implementation for token request synchronization primitive
/// </summary>
public class AccessTokenRequestSynchronization : IUserAccessTokenRequestSynchronization
{
    /// <inheritdoc />
    public ConcurrentDictionary<string, Lazy<Task<string>>> Dictionary { get; } = new();
}