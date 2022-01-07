using Microsoft.AspNetCore.Http;

namespace BudgetCast.Common.Web.Middleware
{
    public class CurrentTenantMiddleware : IMiddleware
    {
        private readonly ITenantService _tenantService;

        public CurrentTenantMiddleware(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!ExcludePath(context))
            {
                string? tenantId = TenantResolver.Resolver(context);
                if (!string.IsNullOrEmpty(tenantId))
                {
                    _tenantService.SetCurrentTenant(long.Parse(tenantId));
                }
                else
                {
                    throw new Exception("Tenant is not identifieable");
                }
            }

            await next(context);
        }

        private static bool ExcludePath(HttpContext context)
        {
            var listExclude = new List<string>()
            {
                "/swagger",
                "/jobs"
            };

            foreach (string item in listExclude)
            {
                if (context.Request.Path.StartsWithSegments(item))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
