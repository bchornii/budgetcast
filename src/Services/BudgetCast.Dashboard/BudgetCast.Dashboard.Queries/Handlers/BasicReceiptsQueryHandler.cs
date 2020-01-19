using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Domain.ReadModel.Campaign;
using BudgetCast.Dashboard.Domain.ReadModel.General;
using BudgetCast.Dashboard.Domain.ReadModel.Receipts;
using BudgetCast.Dashboard.Queries.Queries;
using BudgetCast.Dashboard.Queries.Results;
using MediatR;
using static BudgetCast.Dashboard.Queries.Results
    .QueryResult<BudgetCast.Dashboard.Domain.ReadModel.General
        .PageResult<BudgetCast.Dashboard.Domain.ReadModel.Receipts.BasicReceipt>>;

namespace BudgetCast.Dashboard.Queries.Handlers
{
    public class BasicReceiptsQueryHandler : IRequestHandler<
        BasicReceiptsQuery, QueryResult<PageResult<BasicReceipt>>>
    {
        private readonly ICampaignReadAccessor _campaignReadAccessor;
        private readonly IReceiptReadAccessor _receiptReadAccessor;

        public BasicReceiptsQueryHandler(
            ICampaignReadAccessor campaignReadAccessor, 
            IReceiptReadAccessor receiptReadAccessor)
        {
            _campaignReadAccessor = campaignReadAccessor;
            _receiptReadAccessor = receiptReadAccessor;
        }

        public async Task<QueryResult<PageResult<BasicReceipt>>> Handle(
            BasicReceiptsQuery request, 
            CancellationToken cancellationToken)
        {
            var campaignId = await _campaignReadAccessor
                .GetIdByName(request.CampaignName);
            var result = await _receiptReadAccessor
                .GetBasicReceipts(campaignId, request.Page, request.PageSize);
            
            return GetSuccessResult(result);
        }
    }
}
