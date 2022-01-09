using FluentValidation;

namespace BudgetCast.Expenses.Queries.Expenses.GetExpenseById
{
    public class GetExpenseByIdQueryValidator : AbstractValidator<GetExpenseByIdQuery>
    {
        public GetExpenseByIdQueryValidator()
        {
            RuleFor(x => x.ExpenseId)
                .NotEmpty();
        }
    }
}
