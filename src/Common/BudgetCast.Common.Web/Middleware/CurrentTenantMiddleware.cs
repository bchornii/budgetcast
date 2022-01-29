using BudgetCast.Common.Authentication;
using Microsoft.AspNetCore.Http;

namespace BudgetCast.Common.Web.Middleware
{
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
                    throw new Exception("Tenant is not identifieable");
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
