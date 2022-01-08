using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Queries;

namespace BudgetCast.Expenses.Queries.Campaigns.GetCampaignByName
{
    public record GetCampaignByNameQuery : IQuery<Result<CampaignVm>>
    {
        public string Name { get; init; }

        public GetCampaignByNameQuery()
        {
            Name = default!;
        }
    }

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
            var result = await _campaignDataAccess.GetAsync(request.Name, cancellationToken);
            return new Success<CampaignVm>(result);
        }
    }
}
