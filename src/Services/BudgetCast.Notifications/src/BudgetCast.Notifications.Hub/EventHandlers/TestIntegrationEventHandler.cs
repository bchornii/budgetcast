using BudgetCast.Common.Authentication;
using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Notifications.AppHub.Hubs;
using BudgetCast.Notifications.AppHub.Models;
using BudgetCast.Notifications.AppHub.Services;

namespace BudgetCast.Notifications.AppHub.EventHandlers;

public class TestIntegrationEventHandler : IEventHandler<TestIntegrationEvent>
{
    private readonly IIdentityContext _identityContext;
    private readonly ILogger<TestIntegrationEventHandler> _logger;
    private readonly INotificationService _notificationService;

    public TestIntegrationEventHandler(
        IIdentityContext identityContext, 
        ILogger<TestIntegrationEventHandler> logger, 
        INotificationService notificationService)
    {
        _identityContext = identityContext;
        _logger = logger;
        _notificationService = notificationService;
    }
    
    public async Task Handle(TestIntegrationEvent @event, CancellationToken cancellationToken)
    {
        var groupName = $"{NotificationHub.GroupPrefix}-{_identityContext.TenantId}";

        _logger.LogInformationIfEnabled("Sending notification to {GroupName}", groupName);
        
        await _notificationService.SendMessageToGroupAsync(
            notification: new GeneralNotification
            {
                Type = NotificationType.Success,
                Message = $"Hi, group {groupName}",
                MessageType = NotificationMessageTypes.ExpensesAdded,
                Data = new
                {
                    Id = Guid.NewGuid(),
                    Total = 1023.0,
                    AddedBy = "bchornii",
                    AddedAt = DateTime.Now
                }
            },
            @group: groupName, cancellationToken: CancellationToken.None);
        
        _logger.LogInformationIfEnabled("Notification to {GroupName} has been successfully sent", groupName);
    }
}