using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Queries;
using BudgetCast.Expenses.Queries.Expenses;

namespace BudgetCast.Expenses.Queries.Campaigns.GetCampaignTotals
{
    public record GetCampaignTotalsQuery(string Name) :
        IQuery<Result<TotalsPerCampaignVm>>;

    public class GetCampaignTotalsQueryHandler :
        IQueryHandler<GetCampaignTotalsQuery, Result<TotalsPerCampaignVm>>
    {
        private readonly IExpensesDataAccess _expensesDataAccess;

        public GetCampaignTotalsQueryHandler(IExpensesDataAccess expensesDataAccess)
        {
            _expensesDataAccess = expensesDataAccess;
        }

        public async Task<Result<TotalsPerCampaignVm>> Handle(
            GetCampaignTotalsQuery request,
            CancellationToken cancellationToken)
        {
            return await _expensesDataAccess.GetTotalsAsync(request.Name, cancellationToken);
        }
    }
}
