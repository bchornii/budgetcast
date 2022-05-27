using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Command;
using BudgetCast.Common.Domain;
using BudgetCast.Common.Domain.Results;
using BudgetCast.Expenses.Commands.Expenses;
using BudgetCast.Expenses.Domain.Expenses;

namespace BudgetCast.Expenses.Commands.Tags
{
    public record UpdateExpenseTagsCommand(long ExpenseId, string[] Tags) : 
        ICommand<Result>;

    public class UpdateExpenseTagsCommandHandler 
        : ICommandHandler<UpdateExpenseTagsCommand, Result>
    {
        private readonly IExpensesRepository _expensesRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateExpenseTagsCommandHandler(
            IExpensesRepository expensesRepository,
            IUnitOfWork unitOfWork) 
        {
            _expensesRepository = expensesRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(
            UpdateExpenseTagsCommand request, 
            CancellationToken cancellationToken)
        {
            var expense = await _expensesRepository
                .GetAsync(request.ExpenseId, cancellationToken);
            var tags = Mapper.MapFrom(request.Tags);
            expense.AddTags(tags);
            await _unitOfWork.Commit(cancellationToken);
            return Success.Empty;
        }
    }
}
