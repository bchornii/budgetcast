using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace BudgetCast.Gateways.Bff.Models;

public static class AppConstants
{
    public const string DefaultAppAuthScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    public const string TokenPrefix = ".Token.";
    public const string AccessTokenName = $"{TokenPrefix}{OpenIdConnectParameterNames.AccessToken}";
    public const string ExpiresAtName = $"{TokenPrefix}expires_at";
    
    /// <summary>
    /// Constants used for YARP
    /// </summary>
    public static class Yarp
    {
        /// <summary>
        /// Name of toke type metadata
        /// </summary>
        public const string TokenTypeMetadata = "BudgetCast.Gw.Bff.Yarp.TokenType";
    }
}