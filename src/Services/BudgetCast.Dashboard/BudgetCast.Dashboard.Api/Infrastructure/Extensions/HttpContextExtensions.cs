using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BudgetCast.Dashboard.Api.Infrastructure.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            return httpContext?.User?.Claims?
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
