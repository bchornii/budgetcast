using BudgetCast.Dashboard.Domain.ReadModel.General;
using BudgetCast.Dashboard.Domain.ReadModel.Receipts;
using BudgetCast.Dashboard.Queries.Results;
using MediatR;

namespace BudgetCast.Dashboard.Queries.Queries
{
    public class BasicReceiptsQuery : IRequest<
        QueryResult<PageResult<BasicReceipt>>>
    {
        public string CampaignName { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
