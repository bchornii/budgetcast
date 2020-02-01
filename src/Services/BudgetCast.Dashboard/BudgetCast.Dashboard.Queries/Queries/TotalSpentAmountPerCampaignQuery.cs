using BudgetCast.Dashboard.Domain.ReadModel.Receipts;
using BudgetCast.Dashboard.Queries.Results;
using MediatR;

namespace BudgetCast.Dashboard.Queries.Queries
{
    public class TotalSpentAmountPerCampaignQuery : IRequest<
        QueryResult<TotalsPerCampaign>>
    {
        public string UserId { get; set; }
        public string CampaignName { get; set; }
    }
}