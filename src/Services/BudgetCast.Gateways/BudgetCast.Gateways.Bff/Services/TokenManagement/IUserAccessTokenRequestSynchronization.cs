using System.Collections.Concurrent;

namespace BudgetCast.Gateways.Bff.Services.TokenManagement;

/// <summary>
/// Service to provide a concurrent dictionary for synchronizing token endpoint requests
/// </summary>
public interface IUserAccessTokenRequestSynchronization
{
    /// <summary>
    /// Concurrent dictionary as synchronization primitive
    /// </summary>
    public ConcurrentDictionary<string, Lazy<Task<string>>> Dictionary { get; }
}