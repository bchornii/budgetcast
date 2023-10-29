using System.Globalization;
using System.Security.Claims;
using BudgetCast.Gateways.Bff.Extensions;
using BudgetCast.Gateways.Bff.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace BudgetCast.Gateways.Bff.Services;

public class AuthenticationSessionUserAccessTokenStore : IUserAccessTokenStore
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<AuthenticationSessionUserAccessTokenStore> _logger;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="contextAccessor"></param>
    /// <param name="logger"></param>
    public AuthenticationSessionUserAccessTokenStore(
        IHttpContextAccessor contextAccessor,
        ILogger<AuthenticationSessionUserAccessTokenStore> logger)
    {
        _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        _logger = logger;
    }
    
    public async Task<UserAccessToken> GetTokenAsync(ClaimsPrincipal user)
    {
        var result = await _contextAccessor.HttpContext!.AuthenticateAsync(AppConstants.DefaultAppAuthScheme);

        if (!result.Succeeded)
        {
            _logger.LogInformation("Cannot authenticate scheme: {scheme}", AppConstants.DefaultAppAuthScheme ?? "default signin scheme");
                
            return null;
        }

        if (result.Properties == null)
        {
            _logger.LogInformation("Authentication result properties are null for scheme: {scheme}",
                AppConstants.DefaultAppAuthScheme ?? "default signin scheme");
                
            return null;
        }
        
        var tokens = result.Properties.Items.Where(i => i.Key.StartsWith(AppConstants.TokenPrefix)).ToList();
        if (tokens == null || !tokens.Any())
        {
            _logger.LogInformation("No tokens found in cookie properties. SaveTokens must be enabled for automatic token refresh.");
                
            return null;
        }
        
        var accessToken = tokens.SingleOrDefault(t => t.Key == AppConstants.AccessTokenName);
        var refreshToken = tokens.SingleOrDefault(t => t.Key == $"{AppConstants.TokenPrefix}{OpenIdConnectParameterNames.RefreshToken}");
        var expiresAt = tokens.SingleOrDefault(t => t.Key == AppConstants.ExpiresAtName);

        DateTimeOffset? dtExpires = null;
        if (expiresAt.Value != null)
        {
            dtExpires = DateTimeOffset.Parse(expiresAt.Value, CultureInfo.InvariantCulture);
        }

        return new UserAccessToken
        {
            AccessToken = accessToken.Value!,
            RefreshToken = refreshToken.Value ?? "Not-passed",
            Expiration = dtExpires
        };
    }
    
    public async Task StoreTokenAsync(ClaimsPrincipal user, string accessToken, DateTimeOffset expiration, string? refreshToken = null)
    {
        var result = await _contextAccessor.HttpContext!.AuthenticateAsync(AppConstants.DefaultAppAuthScheme);
        
        if (!result.Succeeded)
        {
            throw new Exception("Can't store tokens. User is anonymous");
        }
        
        result.Properties.Items[AppConstants.AccessTokenName] = accessToken;
        result.Properties.Items[AppConstants.ExpiresAtName] = expiration.ToString("o", CultureInfo.InvariantCulture);

        if (refreshToken != null)
        {
            result.Properties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken, refreshToken);
        }
        
        var options = _contextAccessor.HttpContext!.RequestServices.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
        var cookieOptions = options.Get(AppConstants.DefaultAppAuthScheme);
        if (result.Properties.AllowRefresh == true ||
            (result.Properties.AllowRefresh == null && cookieOptions.SlidingExpiration)
           )
        {
            // this will allow the cookie to be issued with a new issued (and thus a new expiration)
            result.Properties.IssuedUtc = null;
            result.Properties.ExpiresUtc = null;
        }
        
        await _contextAccessor.HttpContext!.SignInAsync(AppConstants.DefaultAppAuthScheme, user, result.Properties);
    }
}