using BudgetCast.Common.Authentication;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using BudgetCast.Expenses.Messaging;
using BudgetCast.Notifications.AppHub.Models;
using BudgetCast.Notifications.AppHub.Services;
using static BudgetCast.Notifications.AppHub.Hubs.NotificationHub;

namespace BudgetCast.Notifications.AppHub.EventHandlers;

/// <summary>
/// Handles integration events which belong to Expenses area of the system. Transform those events
/// into instances of <see cref="GeneralNotification"/> type and pass down the line to notification service. 
/// </summary>
public class ExpensesEventsHandler : 
    IEventHandler<ExpensesAddedEvent>,
    IEventHandler<ExpensesRemovedEvent>
{
    private readonly IIdentityContext _identityContext;
    private readonly ILogger<ExpensesEventsHandler> _logger;
    private readonly INotificationService _notificationService;

    public ExpensesEventsHandler(
        IIdentityContext identityContext, 
        ILogger<ExpensesEventsHandler> logger, 
        INotificationService notificationService)
    {
        _identityContext = identityContext;
        _logger = logger;
        _notificationService = notificationService;
    }
    
    public async Task Handle(ExpensesAddedEvent @event, CancellationToken cancellationToken)
    {
        var groupName = $"{TenantGroupPrefix}-{_identityContext.TenantId}";

        var notification = new GeneralNotification
        {
            Type = NotificationType.Success,
            Message = $"Expense was successfully added to {@event.CampaignName} campaign.",
            MessageType = NotificationMessageTypes.ExpensesAdded,
            Data = new
            {
                Id = @event.ExpenseId,
                Total = @event.Total,
                AddedBy = @event.AddedBy,
                AddedAt = @event.AddedAt,
                CampaignName = @event.CampaignName,
            }
        };

        await SendNotification(groupName, notification, cancellationToken);
    }

    public async Task Handle(ExpensesRemovedEvent @event, CancellationToken cancellationToken)
    {
        var groupName = $"{TenantGroupPrefix}-{_identityContext.TenantId}";

        var notification = new GeneralNotification
        {
            Type = NotificationType.Success,
            Message = $"Expense was successfully removed from {@event.CampaignName} campaign.",
            MessageType = NotificationMessageTypes.ExpensesRemoved,
            Data = new
            {
                Id = @event.ExpenseId,
                Total = @event.Total,
                AddedBy = @event.AddedBy,
                AddedAt = @event.AddedAt,
                CampaignName = @event.CampaignName,
            }
        };

        await SendNotification(groupName, notification, cancellationToken);
    }

    private async Task SendNotification(
        string groupName, 
        GeneralNotification notification, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformationIfEnabled("Sending notification to {GroupName}", groupName);
        
        await _notificationService.SendMessageToGroupAsync(
            notification: notification,
            @group: groupName, 
            cancellationToken: cancellationToken);
        
        _logger.LogInformationIfEnabled("Notification to {GroupName} has been successfully sent", groupName);
    }
}