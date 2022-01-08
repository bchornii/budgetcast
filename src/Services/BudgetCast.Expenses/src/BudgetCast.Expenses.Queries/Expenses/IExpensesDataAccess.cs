using BudgetCast.Common.Models;
using BudgetCast.Expenses.Queries.Expenses.GetCampaingExpenses;
using BudgetCast.Expenses.Queries.Expenses.GetExpenseById;

namespace BudgetCast.Expenses.Queries.Expenses
{
    public interface IExpensesDataAccess
    {
        Task<PageResult<ExpenseVm>> GetAsync(long campaignId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<IReadOnlyList<string>> SearchForTagsAsync(string tagTerm, int amount);

        Task<ExpenseDetailsVm> GetAsync(long expenseId, CancellationToken cancellationToken);
    }
}
