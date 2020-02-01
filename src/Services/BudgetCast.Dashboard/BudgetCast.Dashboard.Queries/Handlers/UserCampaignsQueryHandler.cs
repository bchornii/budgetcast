using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Domain.ReadModel.Campaigns;
using BudgetCast.Dashboard.Queries.Queries;
using BudgetCast.Dashboard.Queries.Results;
using MediatR;

namespace BudgetCast.Dashboard.Queries.Handlers
{
    public class UserCampaignsQueryHandler : IRequestHandler<UserCampaignsQuery, 
        QueryResult<KeyValuePair<string, string>[]>>
    {
        private readonly ICampaignReadAccessor _campaignReadAccessor;

        public UserCampaignsQueryHandler(ICampaignReadAccessor campaignReadAccessor)
        {
            _campaignReadAccessor = campaignReadAccessor;
        }

        public async Task<QueryResult<KeyValuePair<string, string>[]>> Handle(
            UserCampaignsQuery request, CancellationToken cancellationToken)
        {
            var result = await _campaignReadAccessor.GetCampaigns(request.UserId);
            return QueryResult<KeyValuePair<string, string>[]>.GetSuccessResult(result);
        }
    }
}
