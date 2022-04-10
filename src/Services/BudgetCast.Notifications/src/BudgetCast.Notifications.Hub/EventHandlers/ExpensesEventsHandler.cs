using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Expenses.Messaging;
using BudgetCast.Notifications.AppHub.Application.NotifyExpenseAdded;
using BudgetCast.Notifications.AppHub.Application.NotifyExpenseRemoved;
using BudgetCast.Notifications.AppHub.Models;
using MediatR;

namespace BudgetCast.Notifications.AppHub.EventHandlers;

/// <summary>
/// Handles integration events which belong to Expenses area of the system by dispatching
/// an appropriate commands.
/// </summary>
public class ExpensesEventsHandler : 
    IEventHandler<ExpensesAddedEvent>,
    IEventHandler<ExpensesRemovedEvent>
{
    private readonly IMediator _mediator;

    public ExpensesEventsHandler(IMediator mediator) => _mediator = mediator;

    public async Task Handle(ExpensesAddedEvent @event, CancellationToken cancellationToken) 
        => await _mediator.Send(new NotifyExpenseAddedCommand(@event), cancellationToken);

    public async Task Handle(ExpensesRemovedEvent @event, CancellationToken cancellationToken) 
        => await _mediator.Send(new NotifyExpenseRemovedCommand(@event), cancellationToken);
}