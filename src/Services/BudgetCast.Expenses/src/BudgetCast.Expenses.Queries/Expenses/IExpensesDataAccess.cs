using BudgetCast.Common.Models;
using BudgetCast.Expenses.Queries.Campaigns.GetCampaignTotals;
using BudgetCast.Expenses.Queries.Expenses.GetExpenseById;
using BudgetCast.Expenses.Queries.Expenses.GetExpensesForCampaign;

namespace BudgetCast.Expenses.Queries.Expenses
{
    public interface IExpensesDataAccess
    {
        Task<PageResult<ExpenseVm>> GetAsync(long campaignId, int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<IReadOnlyList<string>> SearchForTagsAsync(string tagTerm, int amount);

        Task<ExpenseDetailsVm> GetAsync(long expenseId, CancellationToken cancellationToken);

        Task<TotalsPerCampaignVm> GetTotalsAsync(string campaignName, CancellationToken cancellationToken);
    }
}
