using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Queries;

namespace BudgetCast.Expenses.Queries.Campaigns.GetCampaigns
{
    public record GetCampaignsQuery : IQuery<Result<IReadOnlyList<CampaignVm>>>;

    public class GetCampaignsQueryHandler :
        IQueryHandler<GetCampaignsQuery, Result<IReadOnlyList<CampaignVm>>>
    {
        private readonly ICampaignDataAccess _campaignDataAccess;

        public GetCampaignsQueryHandler(ICampaignDataAccess campaignDataAccess)
        {
            _campaignDataAccess = campaignDataAccess;
        }

        public async Task<Result<IReadOnlyList<CampaignVm>>> Handle(
            GetCampaignsQuery request, 
            CancellationToken cancellationToken)
        {
            var result = await _campaignDataAccess.GetAsync(cancellationToken);
            return new Success<IReadOnlyList<CampaignVm>>(result);
        }
    }
}
