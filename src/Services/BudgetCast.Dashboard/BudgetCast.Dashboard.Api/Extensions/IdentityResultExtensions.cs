using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace BudgetCast.Dashboard.Api.Extensions
{
    public static class IdentityResultExtensions
    {
        public static string GetErrorMessage(this IdentityResult result)
        {
            return string.Join("; ", result.Errors.Select(r => r.Description));
        }
    }
}
