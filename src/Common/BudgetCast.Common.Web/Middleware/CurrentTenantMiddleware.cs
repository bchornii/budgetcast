using System.Net;
using BudgetCast.Common.Authentication;
using Microsoft.AspNetCore.Http;

namespace BudgetCast.Common.Web.Middleware
{
    /// <summary>
    /// Uses HTTP context (claims principal associated with it) to retrieve TenantId and
    /// updates IdentityContext with it. If tenant can't be identified 401 status code is returned.
    /// <remarks>Not sutable for WebSocket communication based on SignalR</remarks>
    /// </summary>
    public class CurrentTenantMiddleware
    {
        private readonly HashSet<string> _cachedVerifiedExludePaths;

        private readonly RequestDelegate _next;
        private readonly CurrentTenantMiddlewareConfiguration _configuration;

        public CurrentTenantMiddleware(RequestDelegate next, CurrentTenantMiddlewareConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
            _cachedVerifiedExludePaths = new HashSet<string>();
        }

        public async Task InvokeAsync(HttpContext context, IIdentityContext identityContext)
        {
            if (!ExcludePath(context))
            {
                string? tenantId = TenantResolver.Resolver(context);
                if (!string.IsNullOrEmpty(tenantId))
                {
                    identityContext.SetCurrentTenant(long.Parse(tenantId));
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Unable to identify tenant");
                    return;
                }
            }

            await _next(context);
        }

        private bool ExcludePath(HttpContext context)
        {
            if (_cachedVerifiedExludePaths.Contains(context.Request.Path))
            {
                return true;
            }

            foreach (string item in _configuration.PathsToExlude)
            {
                if (context.Request.Path.StartsWithSegments(item, StringComparison.OrdinalIgnoreCase))
                {
                    _cachedVerifiedExludePaths.Add(context.Request.Path);
                    return true;
                }
            }

            return false;
        }
    }
}
