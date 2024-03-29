﻿using BudgetCast.Common.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace BudgetCast.Notifications.AppHub.Hubs
{
    [Authorize]
    public class NotificationHub : BaseAppHub
    {
        public const string TenantGroupPrefix = "TenantGroup";
        
        private readonly ILogger<NotificationHub> _logger;
        private readonly IIdentityContext _identityContext;

        public NotificationHub(ILogger<NotificationHub> logger, IIdentityContext identityContext)
        {
            _logger = logger;
            _identityContext = identityContext;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"{TenantGroupPrefix}-{_identityContext.TenantId}");
            await base.OnConnectedAsync();
            _logger.LogInformation("A client connected to NotificationHub: {ConnectionId}", Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"{TenantGroupPrefix}-{_identityContext.TenantId}");
            await base.OnDisconnectedAsync(exception);
            _logger.LogInformation("A client disconnected from NotificationHub: {ConnectionId}", Context.ConnectionId);
        }
    }
}
