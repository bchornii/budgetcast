using BudgetCast.Common.Web.Middleware;
using Microsoft.AspNetCore.Builder;

namespace BudgetCast.Common.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCurrentTenant(this IApplicationBuilder app) =>
            app.UseMiddleware<CurrentTenantMiddleware>();
    }
}
