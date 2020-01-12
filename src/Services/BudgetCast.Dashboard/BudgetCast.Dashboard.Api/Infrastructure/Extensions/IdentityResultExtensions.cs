using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace BudgetCast.Dashboard.Api.Infrastructure.Extensions
{
    public static class IdentityResultExtensions
    {
        public static string GetErrorMessage(this IdentityResult result)
        {
            return result.Errors.Select(r => r.Description).FirstOrDefault();
        }
    }
}
