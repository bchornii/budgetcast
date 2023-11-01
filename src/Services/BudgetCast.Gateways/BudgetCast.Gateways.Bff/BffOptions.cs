namespace BudgetCast.Gateways.Bff;

public class BffOptions
{
    /// <summary>
    /// Bff authentication cookie name.
    /// </summary>
    public string BffAuthenticationCookieName { get; } = "__BudgetCast-gw-bff";
    
    /// <summary>
    /// Base path for management endpoints. Defaults to "/bff".
    /// </summary>
    public PathString ManagementBasePath { get; set; } = "/bff";

    /// <summary>
    /// Login individual endpoint
    /// </summary>
    public PathString LoginIndividualPath => ManagementBasePath.Add("/login/individual");

    /// <summary>
    /// Logout individual endpoint
    /// </summary>
    public PathString LogoutIndividualPath => ManagementBasePath.Add("/logout/individual");

    /// <summary>
    /// User endpoint
    /// </summary>
    public PathString UserPath => ManagementBasePath.Add("/user-info");

    public PathString XTokenPath => "/api/SignIn/handleExternalLogin";

    /// <summary>
    /// External login authentication token name.
    /// </summary>
    public string XTokenHeaderName => "X-TOKEN";

    /// <summary>
    /// Defines if X-TOKEN should be removed from a response after extrificated.
    /// </summary>
    public bool RemoveXTokenCookieFromResponse { get; } = true;
}