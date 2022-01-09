using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Queries;

namespace BudgetCast.Expenses.Queries.Campaigns.GetCampaignByName
{
    public record GetCampaignByNameQuery(string Name) : IQuery<Result<CampaignVm>>;

    public class GetCampaignByNameQueryHandler : IQueryHandler<GetCampaignByNameQuery, Result<CampaignVm>>
    {
        private readonly ICampaignDataAccess _campaignDataAccess;

        public GetCampaignByNameQueryHandler(ICampaignDataAccess campaignDataAccess)
        {
            _campaignDataAccess = campaignDataAccess;
        }

        public async Task<Result<CampaignVm>> Handle(
            GetCampaignByNameQuery request, 
            CancellationToken cancellationToken)
        {
            return await _campaignDataAccess.GetAsync(request.Name, cancellationToken);
        }
    }
}
