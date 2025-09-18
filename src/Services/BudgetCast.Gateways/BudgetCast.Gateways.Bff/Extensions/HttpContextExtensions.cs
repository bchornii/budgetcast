using System.Globalization;
using System.Security.Claims;
using BudgetCast.Gateways.Bff.Models;
using Microsoft.AspNetCore.Authentication;

namespace BudgetCast.Gateways.Bff.Extensions;

public static class HttpContextExtensions
{
    public static async Task SignInAsCookieAsync(this HttpContext httpContext, string accessToken)
    {
        var claims = new[]
        {
            new Claim("uuid", Guid.NewGuid().ToString("N")),
        };
        var identity = new ClaimsIdentity(claims, AppConstants.DefaultAppAuthScheme);
        var principal = new ClaimsPrincipal(identity);
        var authProperties = new AuthenticationProperties();
        
        var expiration = accessToken.AsJwt()!.ValidTo;
        authProperties.Items[AppConstants.ExpiresAtName] = expiration.ToString("o", CultureInfo.InvariantCulture);
        authProperties.Items[AppConstants.AccessTokenName] = accessToken;
        authProperties.Items[AppConstants.RefreshTokenName] = "Not-passed";
        
        await httpContext.SignInAsync(
            AppConstants.DefaultAppAuthScheme, 
            principal,
            authProperties);
    }
    
    public static bool CheckAntiForgeryHeader(this HttpContext context, BffOptions options)
    {
        var antiForgeryHeader = context.Request.Headers[options.AntiForgeryHeaderName].FirstOrDefault();
        return antiForgeryHeader != null && antiForgeryHeader == options.AntiForgeryHeaderValue;
    }
}