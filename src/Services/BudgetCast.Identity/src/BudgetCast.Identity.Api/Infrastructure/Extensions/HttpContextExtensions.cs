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

        public static string GenerateIpAddress(this HttpContext httpContext)
        {
            if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return httpContext.Request.Headers["X-Forwarded-For"];
            }
            else
            {
                return httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
            }
        }
    }
}
