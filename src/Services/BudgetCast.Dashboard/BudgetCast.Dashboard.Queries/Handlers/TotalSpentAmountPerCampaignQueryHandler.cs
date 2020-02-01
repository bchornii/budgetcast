using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Domain.ReadModel.Campaigns;
using BudgetCast.Dashboard.Domain.ReadModel.Receipts;
using BudgetCast.Dashboard.Queries.Queries;
using BudgetCast.Dashboard.Queries.Results;
using MediatR;

namespace BudgetCast.Dashboard.Queries.Handlers
{
    public class TotalSpentAmountPerCampaignQueryHandler : IRequestHandler<
        TotalSpentAmountPerCampaignQuery, QueryResult<TotalsPerCampaign>>
    {
        private readonly ICampaignReadAccessor _campaignReadAccessor;
        private readonly IReceiptReadAccessor _receiptReadAccessor;

        public TotalSpentAmountPerCampaignQueryHandler(
            ICampaignReadAccessor campaignReadAccessor,
            IReceiptReadAccessor receiptReadAccessor)
        {
            _campaignReadAccessor = campaignReadAccessor;
            _receiptReadAccessor = receiptReadAccessor;
        }

        public async Task<QueryResult<TotalsPerCampaign>> Handle(TotalSpentAmountPerCampaignQuery request, 
            CancellationToken cancellationToken)
        {
            var campaignId = await _campaignReadAccessor.GetIdByName(request.CampaignName);
            var result = await _receiptReadAccessor.GetTotals(campaignId, request.UserId);
            return QueryResult<TotalsPerCampaign>.GetSuccessResult(result);
        }
    }
}
