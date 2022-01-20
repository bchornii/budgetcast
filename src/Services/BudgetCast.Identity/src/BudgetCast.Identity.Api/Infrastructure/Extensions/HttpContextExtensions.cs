using System.Security.Claims;

namespace BudgetCast.Identity.Api.Infrastructure.Extensions
{
    internal static class HttpContextExtensions
    {
        public static string? GetUserId(this HttpContext httpContext)
        {
            return httpContext?.User?.Claims?
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
