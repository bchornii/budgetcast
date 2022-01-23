using Microsoft.AspNetCore.Identity;

namespace BudgetCast.Identity.Api.Infrastructure.Extensions
{
    internal static class IdentityResultExtensions
    {
        public static string? GetErrorMessage(this IdentityResult result)
        {
            return result.Errors.Select(r => r.Description).FirstOrDefault();
        }
    }
}
