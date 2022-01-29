using BudgetCast.Common.Web.Middleware;
using Microsoft.AspNetCore.Builder;

namespace BudgetCast.Common.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCurrentTenant(this IApplicationBuilder app, string[]? pathsToExlude = null)
        {
            var configuration = new CurrentTenantMiddlewareConfiguration();
            if (pathsToExlude is not null)
            {
                configuration.AddRange(pathsToExlude);
            }
            return app.UseMiddleware<CurrentTenantMiddleware>(configuration);
        }
    }
}
