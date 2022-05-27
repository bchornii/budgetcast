using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Command;
using BudgetCast.Common.Authentication;
using BudgetCast.Common.Domain.Results;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using BudgetCast.Common.Operations;
using BudgetCast.Expenses.Messaging;
using BudgetCast.Notifications.AppHub.Models;
using BudgetCast.Notifications.AppHub.Services;
using static BudgetCast.Notifications.AppHub.Hubs.NotificationHub;

namespace BudgetCast.Notifications.AppHub.Application.NotifyExpenseRemoved;

public record NotifyExpenseRemovedCommand(ExpensesRemovedEvent ExpensesRemovedEvent) : ICommand<Result>;

public class NotifyExpenseRemovedCommandHandler : ICommandHandler<NotifyExpenseRemovedCommand, Result>
{
    private readonly IIdentityContext _identityContext;
    private readonly ILogger<NotifyExpenseRemovedCommandHandler> _logger;
    private readonly INotificationService _notificationService;
    private readonly OperationContext _operationContext;

    public NotifyExpenseRemovedCommandHandler(
        IIdentityContext identityContext, 
        ILogger<NotifyExpenseRemovedCommandHandler> logger, 
        INotificationService notificationService,
        OperationContext operationContext)
    {
        _identityContext = identityContext;
        _logger = logger;
        _notificationService = notificationService;
        _operationContext = operationContext;
    }

    public async Task<Result> Handle(NotifyExpenseRemovedCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message path: {path}", _operationContext.GetDescription());

        var @event = request.ExpensesRemovedEvent;
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
        
        _logger.LogInformationIfEnabled("Sending notification to {GroupName}", groupName);
        
        await _notificationService.SendMessageToGroupAsync(
            notification: notification,
            @group: groupName, 
            cancellationToken: cancellationToken);
        
        _logger.LogInformationIfEnabled("Notification to {GroupName} has been successfully sent", groupName);
        
        return Success.Empty;
    }
}