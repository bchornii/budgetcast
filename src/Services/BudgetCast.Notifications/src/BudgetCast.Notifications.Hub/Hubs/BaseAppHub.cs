using BudgetCast.Notifications.AppHub.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;

namespace BudgetCast.Notifications.AppHub.Hubs
{
    public class BaseAppHub : Hub
    {
        protected virtual async Task OnHeartbeatCheckTokenExpiration()
        {
            var transportFeature = Context.Features.Get<IHttpTransportFeature>();

            if (transportFeature == null)
            {
                return;
            }

            // No need for LongPolling as request are being sent over to server over and over
            // with Authorization header in place; similar with ServerSentEvents
            if (transportFeature.TransportType != HttpTransportType.WebSockets)
            {
                return;
            }

            var feature = Context.Features.Get<IConnectionHeartbeatFeature>();
            if (feature == null)
            {
                return;
            }

            var context = Context.GetHttpContext();
            if (context == null)
            {
                throw new InvalidOperationException("The HTTP context cannot be resolved.");
            }

            // Extract the authentication ticket from the access token.
            // Note: this operation should be cheap as the authentication result
            // was already computed when SignalR invoked the authentication handler
            // and automatically cached by AuthenticationHandler.AuthenticateAsync().
            var result = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
            if (result.Ticket == null)
            {
                Context.Abort();

                return;
            }

            feature.OnHeartbeat(state =>
            {
                var (exp, ctx) = ((DateTime, HubCallerContext))state;

                // Ensure the access token token is still valid.
                // If it's not, abort the connection immediately.
                if (exp < DateTimeOffset.UtcNow)
                {
                    // TODO: uncomment to break connection after token expiration;
                    // TODO: please note, need to add additional logic on client side to handle it properly
                    //ctx.Abort();
                }
            }, (result.Ticket.GetExpiration(), Context));
        }
    }
}
