using BudgetCast.Common.Authentication;
using BudgetCast.Common.Web.Middleware;
using Microsoft.AspNetCore.SignalR;

namespace BudgetCast.Notifications.AppHub.Infrastructure.HubFilters
{
    /// <summary>
    /// On different stages of WebSocket communication tries to retrieve tenant id
    /// from HTTP context claims principal and update IdentityContext with it value.
    /// </summary>
    public class CurrentTenantFilter : IHubFilter
    {
        /// <summary>
        /// Updates IdentityContext with current tenant id before hub methods invoked
        /// by clients.
        /// </summary>
        /// <param name="invocationContext"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext, 
            Func<HubInvocationContext, ValueTask<object?>> next)
        {
            var identityContext = invocationContext.ServiceProvider
                .GetRequiredService<IIdentityContext>();
            var httpContext = invocationContext.Context.GetHttpContext();

            SetTenantFor(identityContext, httpContext!);

            return next(invocationContext);
        }

        /// <summary>
        /// Updates IdentityContext with current tenant id on connection establishment.
        /// </summary>
        /// <param name="lifetimeContext"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Task OnConnectedAsync(
            HubLifetimeContext lifetimeContext, 
            Func<HubLifetimeContext, Task> next)
        {
            var identityContext = lifetimeContext.ServiceProvider
                .GetRequiredService<IIdentityContext>();
            var httpContext = lifetimeContext.Context.GetHttpContext();

            SetTenantFor(identityContext, httpContext!);

            return next(lifetimeContext);
        }

        /// <summary>
        /// Updates IdentityContext with current tenant id on disconnect.
        /// </summary>
        /// <param name="lifetimeContext"></param>
        /// <param name="exception"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public Task OnDisconnectedAsync(
            HubLifetimeContext lifetimeContext, 
            Exception exception, 
            Func<HubLifetimeContext, Exception, Task> next)
        {
            var identityContext = lifetimeContext.ServiceProvider
                .GetRequiredService<IIdentityContext>();
            var httpContext = lifetimeContext.Context.GetHttpContext();

            SetTenantFor(identityContext, httpContext!);

            return next(lifetimeContext, exception);
        }

        private static void SetTenantFor(IIdentityContext identityContext, HttpContext httpContext)
        {
            var tenantId = TenantResolver.Resolver(httpContext);
            if (!string.IsNullOrEmpty(tenantId))
            {
                identityContext.SetCurrentTenant(long.Parse(tenantId));
            }
            else
            {
                throw new Exception("Tenant cannot be identified");
            }
        }
    }
}
