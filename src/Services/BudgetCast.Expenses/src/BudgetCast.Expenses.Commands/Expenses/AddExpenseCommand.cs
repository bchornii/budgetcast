using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Command;
using BudgetCast.Common.Authentication;
using BudgetCast.Common.Domain;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Expenses.Commands.Tags;
using BudgetCast.Expenses.Domain.Campaigns;
using BudgetCast.Expenses.Domain.Expenses;
using BudgetCast.Expenses.Messaging;

namespace BudgetCast.Expenses.Commands.Expenses
{
    public record AddExpenseCommand : ICommand<Result<long>>
    {
        public DateTime AddedAt { get; init; }

        public string[] Tags { get; init; } = default!;

        public string CampaignName { get; init; } = default!;

        public string? Description { get; init; } = default!;

        public decimal TotalAmount { get; set; }
    }

    public class AddExpenseCommandHandler : 
        ICommandHandler<AddExpenseCommand, Result<long>>
    {
        private readonly IExpensesRepository _expensesRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventsPublisher _eventsPublisher;
        private readonly IIdentityContext _identityContext;

        public AddExpenseCommandHandler(
            IExpensesRepository expensesRepository, 
            ICampaignRepository  campaignRepository,
            IUnitOfWork unitOfWork,
            IEventsPublisher eventsPublisher, 
            IIdentityContext identityContext)
        {
            _expensesRepository = expensesRepository;
            _campaignRepository = campaignRepository;
            _unitOfWork = unitOfWork;
            _eventsPublisher = eventsPublisher;
            _identityContext = identityContext;
        }

        public async Task<Result<long>> Handle(
            AddExpenseCommand request, 
            CancellationToken cancellationToken)
        {
            var campaign = await _campaignRepository
                .GetByNameAsync(request.CampaignName, cancellationToken);

            if(campaign is null)
            {
                var newCampaign = new Campaign(request.CampaignName);
                campaign = await _campaignRepository.AddAsync(newCampaign, cancellationToken);
            }

            var expense = new Expense(request.AddedAt, campaign, request.Description);

            var tags = Mapper.MapFrom(request.Tags);
            expense.AddTags(tags);

            var expenseItem = new ExpenseItem("Default item", request.TotalAmount);
            expense.AddItem(expenseItem);
            await _expensesRepository.AddAsync(expense, cancellationToken);
            
            // TODO: implement outbox pattern to have consistency between storage & message broker
            await _unitOfWork.Commit(cancellationToken);

            var expenseAddedEvent = new ExpensesAddedEvent(
                tenantId: _identityContext.TenantId!.Value,
                expenseId: expense.Id,
                total: expense.GetTotalAmount(),
                addedBy: expense.CreatedBy,
                addedAt: expense.AddedAt,
                campaignName: campaign.Name);
            await _eventsPublisher.Publish(expenseAddedEvent, cancellationToken);

            return expense.Id;
        }
    }
}
