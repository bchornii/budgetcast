using BudgetCast.Common.Web.Constants;
using BudgetCast.Common.Web.Extensions;
using Microsoft.AspNetCore.Http;

namespace BudgetCast.Common.Web.Middleware
{
    public static class TenantResolver
    {
        public static string? Resolver(HttpContext context)
        {
            string? tenantId = ResolveFromUserAuth(context);
            if (!string.IsNullOrEmpty(tenantId))
            {
                return tenantId;
            }

            tenantId = ResolveFromHeader(context);
            if (!string.IsNullOrEmpty(tenantId))
            {
                return tenantId;
            }

            tenantId = ResolveFromQuery(context);
            if (!string.IsNullOrEmpty(tenantId))
            {
                return tenantId;
            }

            return default;
        }

        private static string? ResolveFromUserAuth(HttpContext context) =>
            context.User.GetTenant();

        [Obsolete("Tenant ID should be retrieved from auth ticket", error: false)]
        private static string? ResolveFromHeader(HttpContext context) =>
            context.Request.Headers.TryGetValue(HeaderConstants.Tenant, out var tenantFromHeader) ? (string)tenantFromHeader : default;

        [Obsolete("Tenant ID should be retrieved from auth ticket", error: false)]
        private static string? ResolveFromQuery(HttpContext context) =>
            context.Request.Query.TryGetValue(QueryConstants.Tenant, out var tenantFromQueryString) ? (string)tenantFromQueryString : default;
    }
}
