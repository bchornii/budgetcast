using BudgetCast.Common.Authentication;
using BudgetCast.Notifications.AppHub.Hubs;
using BudgetCast.Notifications.AppHub.Models;
using Microsoft.AspNetCore.SignalR;

namespace BudgetCast.Notifications.AppHub.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IIdentityContext _identityContext;

        public NotificationService(
            IHubContext<NotificationHub> hubContext, 
            IIdentityContext identityContext)
        {
            _hubContext = hubContext;
            _identityContext = identityContext;
        }

        #region RootTenantMethods

        public async Task BroadcastMessageAsync(INotificationMessage notification)
        {
            await _hubContext
                .Clients
                .All
                .SendAsync(notification.MessageType, notification);
        }

        public async Task BroadcastExceptMessageAsync(
            INotificationMessage notification, 
            IEnumerable<string> excludedConnectionIds)
        {
            await _hubContext
                .Clients
                .AllExcept(excludedConnectionIds)
                .SendAsync(notification.MessageType, notification);
        }

        #endregion RootTenantMethods

        public async Task SendMessageAsync(INotificationMessage notification)
        {
            var tenant = _identityContext.TenantId;
            await _hubContext
                .Clients
                .Group($"GroupTenant-{tenant}")
                .SendAsync(notification.MessageType, notification);
        }

        public async Task SendMessageExceptAsync(
            INotificationMessage notification, 
            IEnumerable<string> excludedConnectionIds)
        {
            var tenant = _identityContext.TenantId;
            await _hubContext
                .Clients
                .GroupExcept($"GroupTenant-{tenant}", excludedConnectionIds)
                .SendAsync(notification.MessageType, notification);
        }

        public async Task SendMessageToGroupAsync(
            INotificationMessage notification, 
            string group)
        {
            await _hubContext
                .Clients
                .Group(group)
                .SendAsync(notification.MessageType, notification);
        }

        public async Task SendMessageToGroupsAsync(
            INotificationMessage notification, 
            IEnumerable<string> groupNames)
        {
            await _hubContext
                .Clients
                .Groups(groupNames)
                .SendAsync(notification.MessageType, notification);
        }

        public async Task SendMessageToGroupExceptAsync(
            INotificationMessage notification, 
            string group, 
            IEnumerable<string> excludedConnectionIds)
        {
            await _hubContext
                .Clients
                .GroupExcept(group, excludedConnectionIds)
                .SendAsync(notification.MessageType, notification);
        }

        public async Task SendMessageToUserAsync(
            string userId, 
            INotificationMessage notification)
        {
            await _hubContext
                .Clients
                .User(userId)
                .SendAsync(notification.MessageType, notification);
        }

        public async Task SendMessageToUsersAsync(
            IEnumerable<string> userIds, 
            INotificationMessage notification)
        {
            await _hubContext
                .Clients
                .Users(userIds)
                .SendAsync(notification.MessageType, notification);
        }
    }
}
