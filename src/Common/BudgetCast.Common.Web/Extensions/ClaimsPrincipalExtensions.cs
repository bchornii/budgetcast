using BudgetCast.Common.Web.Constants;
using System.Security.Claims;

namespace BudgetCast.Common.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetTenant(this ClaimsPrincipal principal) =>
            principal.Claims.FirstOrDefault(c => c.Type == ClaimConstants.Tenant)?.Value;
    }
}
