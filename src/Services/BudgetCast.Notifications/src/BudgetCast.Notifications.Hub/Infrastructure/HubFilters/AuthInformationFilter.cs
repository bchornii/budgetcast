using BudgetCast.Common.Authentication;
using BudgetCast.Common.Web.Extensions;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BudgetCast.Notifications.AppHub.Infrastructure.HubFilters
{
    /// <summary>
    /// In Azure SignalR service scenario in Default mode, client connection is proxied
    /// from SignalR service over one of the server connections established with SignalR service (by default 5).
    /// In this scenario when client connects HttpContext is not accessible during <see cref="IIdentityContext"/>
    /// construction in DI, but it is passed over the WebSocket connection and may be retrieved in a filter.
    /// To overcome the issue <see cref="AuthInformationFilter"/> will supplement
    /// any information missing to proceed with normal flow of <see cref="IdentityContext"/> instance.
    /// </summary>
    /// <remarks>Please not that this filter is only required when Azure SignalR is used to host
    /// WebSocket connections. In self-hosting mode, when the app server hosts them, this filter should
    /// not be applied.
    /// </remarks>
    public class AuthInformationFilter : IHubFilter
    {
        /// <summary>
        /// Updates user information with data from HttpContext.User object.
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

            if (identityContext == IdentityContext.NotConstructed && httpContext is not null)
            {
                var principal = httpContext.User;
                if (!principal.IsAnyIdentityAuthenticated())
                {
                    return next(lifetimeContext);
                }

                var userId = principal.Claims
                    .First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                identityContext.SetUserId(userId);
            }

            return next(lifetimeContext);
        }
    }
}
