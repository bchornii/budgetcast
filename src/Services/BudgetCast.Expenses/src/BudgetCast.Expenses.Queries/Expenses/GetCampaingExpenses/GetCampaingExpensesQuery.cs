using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Queries;
using BudgetCast.Common.Models;
using BudgetCast.Expenses.Queries.Campaigns;

namespace BudgetCast.Expenses.Queries.Expenses.GetCampaingExpenses
{
    public record GetCampaingExpensesQuery : 
        IQuery<Result<PageResult<ExpenseVm>>>
    {
        public string CampaignName { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public GetCampaingExpensesQuery()
        {
            CampaignName = default!;
        }
    }

    public class GetCampaingExpensesQueryHandler : 
        IQueryHandler<GetCampaingExpensesQuery, Result<PageResult<ExpenseVm>>>
    {
        private readonly IExpensesDataAccess _expensesDataAccess;
        private readonly ICampaignDataAccess _campaignDataAccess;

        public GetCampaingExpensesQueryHandler(
            IExpensesDataAccess expensesDataAccess, 
            ICampaignDataAccess campaignDataAccess)
        {
            _expensesDataAccess = expensesDataAccess;
            _campaignDataAccess = campaignDataAccess;
        }

        public async Task<Result<PageResult<ExpenseVm>>> Handle(
            GetCampaingExpensesQuery request, 
            CancellationToken cancellationToken)
        {
            var campaignVm = await _campaignDataAccess.GetAsync(request.CampaignName, cancellationToken);

            return await _expensesDataAccess
                .GetAsync(campaignVm.Id, request.Page, request.PageSize, cancellationToken);
        }
    }
}
