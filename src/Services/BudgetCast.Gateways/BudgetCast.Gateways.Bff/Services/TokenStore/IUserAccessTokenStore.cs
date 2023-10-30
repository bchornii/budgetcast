using System.Security.Claims;

namespace BudgetCast.Gateways.Bff.Services.TokenStore;

public interface IUserAccessTokenStore
{
    /// <summary>
    /// Stores tokens
    /// </summary>
    /// <param name="user">User the tokens belong to</param>
    /// <param name="accessToken">The access token</param>
    /// <param name="expiration">The access token expiration</param>
    /// <param name="refreshToken">The refresh token (optional)</param>
    /// <param name="parameters">Extra optional parameters</param>
    /// <returns></returns>
    Task StoreTokenAsync(ClaimsPrincipal user, string accessToken, DateTimeOffset expiration, string? refreshToken = null);

    /// <summary>
    /// Retrieves tokens from store
    /// </summary>
    /// <param name="user">User the tokens belong to</param>
    /// <param name="parameters">Extra optional parameters</param>
    /// <returns>access and refresh token and access token expiration</returns>
    Task<UserAccessToken> GetTokenAsync(ClaimsPrincipal user);
}