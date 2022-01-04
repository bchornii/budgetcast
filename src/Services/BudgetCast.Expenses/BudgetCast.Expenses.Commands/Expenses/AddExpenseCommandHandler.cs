using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Command;
using BudgetCast.Common.Domain;
using BudgetCast.Expenses.Commands.Tags;
using BudgetCast.Expenses.Domain.Expenses;

namespace BudgetCast.Expenses.Commands.Expenses
{
    public record AddExpenseCommand : ICommand<Result>
    {
        public DateTime AddedAt { get; init; }

        public TagDto[] Tags { get; init; }

        public string CampaignId { get; init; }

        public string Description { get; init; }

        public decimal TotalAmount { get; set; }

        public AddExpenseCommand()
        {
            Tags = default!;
            CampaignId = default!;
            Description = default!;
        }
    }

    public class AddExpenseCommandHandler : ICommandHandler<AddExpenseCommand, Result>
    {
        private readonly IExpensesRepository _expensesRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddExpenseCommandHandler(IExpensesRepository expensesRepository, IUnitOfWork unitOfWork)
        {
            _expensesRepository = expensesRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(
            AddExpenseCommand request, 
            CancellationToken cancellationToken)
        {
            var tags = ExpensesTagsCommandMapper.MapFrom(request.Tags);
            var expenseItem = new ExpenseItem("Default item", request.TotalAmount);
            var expense = new Expense(request.AddedAt, request.CampaignId, request.Description);
            expense.AddTags(tags);
            expense.AddItem(expenseItem);
            await _expensesRepository.Add(expense);
            await _unitOfWork.Commit();
            return new Success();
        }
    }
}
