using BudgetCast.Common.Authentication;
using BudgetCast.Common.Web.Middleware;
using Microsoft.AspNetCore.Authorization;

namespace BudgetCast.Notifications.AppHub.Hubs
{
    [Authorize]
    public class NotificationHub : BaseAppHub
    {
        private readonly ILogger<NotificationHub> _logger;
        private readonly IIdentityContext _identityContext;

        public NotificationHub(ILogger<NotificationHub> logger, IIdentityContext identityContext)
        {
            _logger = logger;
            _identityContext = identityContext;
        }

        public override async Task OnConnectedAsync()
        {
            // TODO: uncomment
            // await OnHeartbeatCheckTokenExpiration();

            var tenant = _identityContext.TenantId;

            await Groups.AddToGroupAsync(Context.ConnectionId, $"GroupTenant-{tenant}");
            await base.OnConnectedAsync();
            _logger.LogInformation($"A client connected to NotificationHub: {Context.ConnectionId}");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var tenant = _identityContext.TenantId;

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"GroupTenant-{tenant}");
            await base.OnDisconnectedAsync(exception);
            _logger.LogInformation($"A client disconnected from NotificationHub: {Context.ConnectionId}");
        }        
    }
}
