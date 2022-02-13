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

        public async Task BroadcastMessageAsync(
            INotificationMessage notification, 
            CancellationToken cancellationToken)
        {
            await _hubContext
                .Clients
                .All
                .SendAsync(notification.Target, notification, cancellationToken: cancellationToken);
        }

        public async Task BroadcastExceptMessageAsync(
            INotificationMessage notification,
            IEnumerable<string> excludedConnectionIds, 
            CancellationToken cancellationToken)
        {
            await _hubContext
                .Clients
                .AllExcept(excludedConnectionIds)
                .SendAsync(notification.Target, notification, cancellationToken: cancellationToken);
        }

        #endregion RootTenantMethods

        public async Task SendMessageAsync(
            INotificationMessage notification, 
            CancellationToken cancellationToken)
        {
            var tenant = _identityContext.TenantId;
            await _hubContext
                .Clients
                .Group($"GroupTenant-{tenant}")
                .SendAsync(notification.Target, notification, cancellationToken: cancellationToken);
        }

        public async Task SendMessageExceptAsync(
            INotificationMessage notification,
            IEnumerable<string> excludedConnectionIds, 
            CancellationToken cancellationToken)
        {
            var tenant = _identityContext.TenantId;
            await _hubContext
                .Clients
                .GroupExcept($"GroupTenant-{tenant}", excludedConnectionIds)
                .SendAsync(notification.Target, notification, cancellationToken: cancellationToken);
        }

        public async Task SendMessageToGroupAsync(
            INotificationMessage notification,
            string @group, 
            CancellationToken cancellationToken)
        {
            await _hubContext
                .Clients
                .Group(group)
                .SendAsync(notification.Target, notification, cancellationToken: cancellationToken);
        }

        public async Task SendMessageToGroupsAsync(
            INotificationMessage notification,
            IEnumerable<string> groupNames, 
            CancellationToken cancellationToken)
        {
            await _hubContext
                .Clients
                .Groups(groupNames)
                .SendAsync(notification.Target, notification, cancellationToken: cancellationToken);
        }

        public async Task SendMessageToGroupExceptAsync(
            INotificationMessage notification,
            string @group,
            IEnumerable<string> excludedConnectionIds, 
            CancellationToken cancellationToken)
        {
            await _hubContext
                .Clients
                .GroupExcept(group, excludedConnectionIds)
                .SendAsync(notification.Target, notification, cancellationToken: cancellationToken);
        }

        public async Task SendMessageToUserAsync(
            string userId,
            INotificationMessage notification, 
            CancellationToken cancellationToken)
        {
            await _hubContext
                .Clients
                .User(userId)
                .SendAsync(notification.Target, notification, cancellationToken: cancellationToken);
        }

        public async Task SendMessageToUsersAsync(
            IEnumerable<string> userIds,
            INotificationMessage notification, 
            CancellationToken cancellationToken)
        {
            await _hubContext
                .Clients
                .Users(userIds)
                .SendAsync(notification.Target, notification, cancellationToken: cancellationToken);
        }
    }
}
