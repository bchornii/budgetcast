using Microsoft.Extensions.Caching.Distributed;

namespace BudgetCast.Gateways.Bff.Services.Session;

public class SessionTrackerService : ISessionTrackerService
{
    private readonly IDistributedCache _distributedCache;

    public SessionTrackerService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task RevokeAsync(string uuid, CancellationToken cancellationToken = default)
        => await _distributedCache.SetStringAsync(uuid, $"revoke-{uuid}", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(14)
        }, cancellationToken);

    public async Task<bool> IsActiveAsync(string uuid, CancellationToken cancellationToken = default)
        => !await IsRevokedAsync(uuid, cancellationToken);

    public async Task<bool> IsRevokedAsync(string uuid, CancellationToken cancellationToken = default)
    {
        var value = await _distributedCache.GetStringAsync(uuid, cancellationToken);
        return !string.IsNullOrWhiteSpace(value);
    }
}