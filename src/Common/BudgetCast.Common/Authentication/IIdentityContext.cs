using System.Security.Claims;

namespace BudgetCast.Common.Authentication
{
    public interface IIdentityContext
    {
        // ClaimsPrincipal is used until error logging has implementation based on it.
        ClaimsPrincipal UserIdentity { get; set; }

        string UserId { get; }
    }
}
