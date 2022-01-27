using BudgetCast.Common.Web.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace BudgetCast.Notifications.AppHub.Hubs
{
    [Authorize]
    public class NotificationHub : BaseAppHub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            // TODO: uncomment
            // await OnHeartbeatCheckTokenExpiration();

            string? tenant = Context.User?.GetTenant();
            if (string.IsNullOrEmpty(tenant))
            {
                throw new Exception();
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"GroupTenant-{tenant}");
            await base.OnConnectedAsync();
            _logger.LogInformation($"A client connected to NotificationHub: {Context.ConnectionId}");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string? tenant = Context.User?.GetTenant();
            if (string.IsNullOrEmpty(tenant))
            {
                throw new Exception();
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"GroupTenant-{tenant}");
            await base.OnDisconnectedAsync(exception);
            _logger.LogInformation($"A client disconnected from NotificationHub: {Context.ConnectionId}");
        }        
    }
}
