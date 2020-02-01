using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Domain.ReadModel.Campaigns;
using BudgetCast.Dashboard.Queries.Queries;
using BudgetCast.Dashboard.Queries.Results;
using MediatR;
using static BudgetCast.Dashboard.Queries.Results.QueryResult<
    System.Collections.Generic.IReadOnlyList<string>>;

namespace BudgetCast.Dashboard.Queries.Handlers
{
    public class DefaultCampaignsQueryHandler : IRequestHandler<
        DefaultCampaignsQuery, QueryResult<IReadOnlyList<string>>>
    {
        private readonly ICampaignReadAccessor _campaignReadAccessor;

        public DefaultCampaignsQueryHandler(ICampaignReadAccessor campaignReadAccessor)
        {
            _campaignReadAccessor = campaignReadAccessor;
        }

        public async Task<QueryResult<IReadOnlyList<string>>> Handle(
            DefaultCampaignsQuery request, CancellationToken cancellationToken)
        {
            var campaigns = await _campaignReadAccessor
                .GetCampaigns(request.Term, request.Amount);

            return GetSuccessResult(campaigns);
        }
    }
}
