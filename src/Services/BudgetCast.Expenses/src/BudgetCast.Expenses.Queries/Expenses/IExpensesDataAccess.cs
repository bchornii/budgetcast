using BudgetCast.Common.Models;
using BudgetCast.Expenses.Queries.Expenses.GetCampaingExpenses;

namespace BudgetCast.Expenses.Queries.Expenses
{
    public interface IExpensesDataAccess
    {
        Task<PageResult<ExpenseVm>> GetAsync(long campaignId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}
