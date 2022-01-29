namespace BudgetCast.Identity.Api.Infrastructure.AppSettings
{
    public class TenantConfiguration
    {
        public static readonly string[] PathsToExcludeFromTenantVerification = new[]
        {
            "/api/signin",
            "/api/account/register",
            "/api/account/email/confirm",
            "/api/account/password/reset",
            "/api/account/password/forgot",
            "/api/account/isAuthenticated",
        };
    }
}
