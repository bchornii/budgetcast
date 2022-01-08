using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Queries;

namespace BudgetCast.Expenses.Queries.Campaigns.FindForExistingCampaignsByName
{
    public record FindForExistingCampaignsByNameQuery(int Amount, string CampaignNameTerm) : IQuery<Result<string[]>>;

    public class FindForExistingCampaignsByNameQueryHandler :
        IQueryHandler<FindForExistingCampaignsByNameQuery, Result<string[]>>
    {
        private readonly ICampaignDataAccess _campaignDataAccess;

        public FindForExistingCampaignsByNameQueryHandler(ICampaignDataAccess campaignDataAccess)
        {
            _campaignDataAccess = campaignDataAccess;
        }

        public async Task<Result<string[]>> Handle(FindForExistingCampaignsByNameQuery request, CancellationToken cancellationToken)
        {
            var result = await _campaignDataAccess
                .GetAsync(request.Amount, request.CampaignNameTerm, cancellationToken);
            return new Success<string[]>(result.Select(r => r.Name).ToArray());
        }
    }
}
