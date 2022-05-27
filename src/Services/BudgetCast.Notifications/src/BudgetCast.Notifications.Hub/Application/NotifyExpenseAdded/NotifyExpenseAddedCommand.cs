using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Command;
using BudgetCast.Common.Authentication;
using BudgetCast.Common.Domain.Results;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using BudgetCast.Expenses.Messaging;
using BudgetCast.Notifications.AppHub.Models;
using BudgetCast.Notifications.AppHub.Services;
using static BudgetCast.Notifications.AppHub.Hubs.NotificationHub;

namespace BudgetCast.Notifications.AppHub.Application.NotifyExpenseAdded;

public record NotifyExpenseAddedCommand(ExpensesAddedEvent ExpensesAddedEvent) : ICommand<Result<bool>>;

public class NotifyExpenseAddedCommandHandler : ICommandHandler<NotifyExpenseAddedCommand, Result<bool>>
{
    private readonly IIdentityContext _identityContext;
    private readonly ILogger<NotifyExpenseAddedCommandHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly IEventsPublisher _eventsPublisher;

    public NotifyExpenseAddedCommandHandler(
        IIdentityContext identityContext,
        ILogger<NotifyExpenseAddedCommandHandler> logger, 
        INotificationService notificationService,
        IEventsPublisher eventsPublisher)
    {
        _identityContext = identityContext;
        _logger = logger;
        _notificationService = notificationService;
        _eventsPublisher = eventsPublisher;
    }
    
    public async Task<Result<bool>> Handle(NotifyExpenseAddedCommand request, CancellationToken cancellationToken)
    {
        var @event = request.ExpensesAddedEvent;
        
        // await _eventsPublisher.Publish(
        //     new ExpensesRemovedEvent(
        //         @event.TenantId, 
        //         @event.ExpenseId, 
        //         @event.Total, 
        //         @event.AddedBy, 
        //         @event.AddedAt,
        //         @event.CampaignName), 
        //     cancellationToken);
        //
        // return new Success<bool>(true);
        
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
        
        _logger.LogInformationIfEnabled("Sending notification to {GroupName}", groupName);
        
        await _notificationService.SendMessageToGroupAsync(
            notification: notification,
            @group: groupName, 
            cancellationToken: cancellationToken);
        
        _logger.LogInformationIfEnabled("Notification to {GroupName} has been successfully sent", groupName);
        
        return Success<bool>.Empty;
    }
}