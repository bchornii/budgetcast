using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Queries;

namespace BudgetCast.Expenses.Queries.Expenses.GetExpenseById
{
    public record GetExpenseByIdQuery(long ExpenseId) : 
        IQuery<Result<ExpenseDetailsVm>>;

    public class GetExpenseByIdQueryHandler : 
        IQueryHandler<GetExpenseByIdQuery, Result<ExpenseDetailsVm>>
    {
        private readonly IExpensesDataAccess _expensesDataAccess;

        public GetExpenseByIdQueryHandler(IExpensesDataAccess expensesDataAccess)
        {
            _expensesDataAccess = expensesDataAccess;
        }

        public async Task<Result<ExpenseDetailsVm>> Handle(
            GetExpenseByIdQuery request, 
            CancellationToken cancellationToken)
        {
            return await _expensesDataAccess
                .GetAsync(request.ExpenseId, cancellationToken);
        }
    }
}
