using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Command;
using BudgetCast.Common.Domain;
using BudgetCast.Expenses.Commands.Tags;
using BudgetCast.Expenses.Domain.Campaigns;
using BudgetCast.Expenses.Domain.Expenses;

namespace BudgetCast.Expenses.Commands.Expenses
{
    public record AddExpenseCommand : ICommand<Result<long>>
    {
        public DateTime AddedAt { get; init; }

        public TagDto[] Tags { get; init; }

        public string CampaignName { get; init; }

        public string Description { get; init; }

        public decimal TotalAmount { get; set; }

        public AddExpenseCommand()
        {
            Tags = default!;
            CampaignName = default!;
            Description = default!;
        }
    }

    public class AddExpenseCommandHandler : ICommandHandler<AddExpenseCommand, Result<long>>
    {
        private readonly IExpensesRepository _expensesRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddExpenseCommandHandler(
            IExpensesRepository expensesRepository, 
            ICampaignRepository  campaignRepository,
            IUnitOfWork unitOfWork)
        {
            _expensesRepository = expensesRepository;
            _campaignRepository = campaignRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<long>> Handle(
            AddExpenseCommand request, 
            CancellationToken cancellationToken)
        {
            var campaign = await _campaignRepository
                .GetByNameAsync(request.CampaignName);

            if(campaign is null)
            {
                var newCampaign = new Campaign(request.CampaignName);
                campaign = await _campaignRepository.Add(newCampaign);
            }

            var expense = new Expense(request.AddedAt, campaign, request.Description);

            var tags = ExpensesTagsCommandMapper.MapFrom(request.Tags);
            expense.AddTags(tags);

            var expenseItem = new ExpenseItem("Default item", request.TotalAmount);
            expense.AddItem(expenseItem);

            await _expensesRepository.Add(expense);
            await _unitOfWork.Commit();

            return expense.Id;
        }
    }
}
