using BudgetCast.Common.Application.Command;
using BudgetCast.Common.Authentication;
using BudgetCast.Common.Domain;
using BudgetCast.Common.Domain.Results;
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
        private readonly IIdentityContext _identityContext;
        private readonly IIntegrationEventLogService _eventLogService;

        public AddExpenseCommandHandler(
            IExpensesRepository expensesRepository, 
            ICampaignRepository  campaignRepository,
            IUnitOfWork unitOfWork, 
            IIdentityContext identityContext,
            IIntegrationEventLogService eventLogService)
        {
            _expensesRepository = expensesRepository;
            _campaignRepository = campaignRepository;
            _unitOfWork = unitOfWork;
            _identityContext = identityContext;
            _eventLogService = eventLogService;
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

            var expense = Expense.Create(request.AddedAt, campaign, request.Description).Value;

            var tags = Mapper.MapFrom(request.Tags);
            var addTagsResult = expense.AddTags(tags);

            var expenseItem = ExpenseItem.Create("Default item", request.TotalAmount).Value;
            var addItemResult = expense.AddItem(expenseItem);
            
            if (!addTagsResult || !addItemResult)
            {
                return Result.GeneralFail<long>(addTagsResult, addItemResult);
            }
            
            await _expensesRepository.AddAsync(expense, cancellationToken);
            
            var expenseAddedEvent = new ExpensesAddedEvent(
                tenantId: _identityContext.TenantId!.Value,
                expenseId: expense.Id,
                total: expense.GetTotalAmount(),
                addedBy: expense.CreatedBy,
                addedAt: expense.AddedAt,
                campaignName: campaign.Name);
            await _eventLogService.AddEventAsync(expenseAddedEvent, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);

            return expense.Id;
        }
    }
}
