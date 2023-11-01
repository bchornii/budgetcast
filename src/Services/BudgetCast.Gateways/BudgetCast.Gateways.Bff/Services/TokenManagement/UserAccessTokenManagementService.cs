using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BudgetCast.Gateways.Bff.Extensions;
using BudgetCast.Gateways.Bff.Models;
using BudgetCast.Gateways.Bff.Services.Identity;
using BudgetCast.Gateways.Bff.Services.TokenStore;

namespace BudgetCast.Gateways.Bff.Services.TokenManagement;

public class UserAccessTokenManagementService : IUserAccessTokenManagementService
{
    private readonly IUserAccessTokenStore _userAccessTokenStore;
    private readonly ILogger<UserAccessTokenManagementService> _logger;
    private readonly IIdentityEndpointService _identityEndpointService;
    private readonly IUserAccessTokenRequestSynchronization _sync;

    public UserAccessTokenManagementService(
        IUserAccessTokenStore userAccessTokenStore, 
        ILogger<UserAccessTokenManagementService> logger,
        IIdentityEndpointService identityEndpointService,
        IUserAccessTokenRequestSynchronization sync)
    {
        _userAccessTokenStore = userAccessTokenStore;
        _logger = logger;
        _identityEndpointService = identityEndpointService;
        _sync = sync;
    }
    
    public async Task<string> GetUserAccessTokenAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        if (user == null || !user.Identity!.IsAuthenticated)
        {
            return null!;
        }
        
        var userName = user.FindFirst(JwtClaimTypes.Name)?.Value ?? user.FindFirst(JwtClaimTypes.Subject)?.Value ?? "unknown";
        var userToken = await _userAccessTokenStore.GetTokenAsync(user);
        
        if (userToken == null)
        {
            _logger.LogDebug("No token data found in user token store for user {user}.", userName);
            return null;
        }
            
        if (userToken.AccessToken.IsPresent() && userToken.RefreshToken.IsMissing())
        {
            _logger.LogDebug("No refresh token found in user token store for user {user} / resource {resource}. Returning current access token.", userName, "default");
            return userToken.AccessToken;
        }

        if (userToken.AccessToken.IsMissing() && userToken.RefreshToken.IsPresent())
        {
            _logger.LogDebug(
                "No access token found in user token store for user {user} / resource {resource}. Trying to refresh.",
                userName, "default");
        }
        
        var dtRefresh = DateTimeOffset.MinValue;
        if (userToken.Expiration.HasValue)
        {
            dtRefresh = userToken.Expiration.Value.Subtract(TimeSpan.FromMinutes(5));
        }

        if (dtRefresh < DateTimeOffset.UtcNow)
        {
            _logger.LogDebug("Token for user {user} needs refreshing.", userName);

            var uuid = user.GetUuid();

            if (string.IsNullOrWhiteSpace(uuid))
            {
                _logger.LogWarning("User dont' have valid uuid. Current value is {uuid}. Returning old access token", uuid);
                return userToken.AccessToken;
            }

            try
            {
                // Since multiple request may occur in near the same time for the same user, we must handle concurrency
                // and eliminate refreshing token multiple times.
                return await _sync.Dictionary.GetOrAdd(uuid, _ =>
                {
                    return new Lazy<Task<string>>(async () =>
                    {
                        var newAccessToken = await RefreshUserAccessTokenAsync(user, cancellationToken);
                        return newAccessToken;
                    });
                }).Value;
            }
            finally
            {
                _sync.Dictionary.TryRemove(uuid, out var _);
            }
        }
        
        return userToken.AccessToken;
    }
    
    private async Task<string> RefreshUserAccessTokenAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var userToken = await _userAccessTokenStore.GetTokenAsync(user);
        var response = await _identityEndpointService
            .RefreshAccessTokenAsync(new RefreshTokenDto(userToken.AccessToken), cancellationToken);

        if (!response.IsSuccess)
        {
            _logger.LogError("Error refreshing access token. StatusCode = {statusCode}", (int)response.StatusCode);
            return null!;
        }
        else
        {
            var tokenJwt = response.NewAccessToken.AsJwt();
            await _userAccessTokenStore.StoreTokenAsync(user, response.NewAccessToken, tokenJwt.ValidTo);
            return response.NewAccessToken;
        }
    }
}