using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Domain.ReadModel.Campaigns;
using BudgetCast.Dashboard.Queries.Queries;
using BudgetCast.Dashboard.Queries.Results;
using MediatR;
using static BudgetCast.Dashboard.Queries.Results.QueryResult<string>;

namespace BudgetCast.Dashboard.Queries.Handlers
{
    public class CampaignIdByNameQueryHandler : IRequestHandler<
        CampaignIdByNameQuery, QueryResult<string>>
    {
        private readonly ICampaignReadAccessor _campaignAccessor;

        public CampaignIdByNameQueryHandler(ICampaignReadAccessor campaignAccessor)
        {
            _campaignAccessor = campaignAccessor;
        }

        public async Task<QueryResult<string>> Handle(CampaignIdByNameQuery request, 
            CancellationToken cancellationToken)
        {
            var id = await _campaignAccessor.GetIdByName(request.CampaignName);
            return string.IsNullOrEmpty(id) ? GetNotFoundResult() : GetSuccessResult(id);
        }
    }
}
