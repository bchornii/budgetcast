using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Queries;

namespace BudgetCast.Expenses.Queries.Campaigns.SearchForExistingCampaignsByName
{
    public record SearchForExistingCampaignsByNameQuery(int Amount, string Term) : 
        IQuery<Result<IReadOnlyList<string>>>;

    public class SearchForExistingCampaignsByNameQueryHandler :
        IQueryHandler<SearchForExistingCampaignsByNameQuery, Result<IReadOnlyList<string>>>
    {
        private readonly ICampaignDataAccess _campaignDataAccess;

        public SearchForExistingCampaignsByNameQueryHandler(ICampaignDataAccess campaignDataAccess)
        {
            _campaignDataAccess = campaignDataAccess;
        }

        public async Task<Result<IReadOnlyList<string>>> Handle(
            SearchForExistingCampaignsByNameQuery request, 
            CancellationToken cancellationToken)
        {
            var result = await _campaignDataAccess
                .GetAsync(request.Amount, request.Term, cancellationToken);
            return new Success<IReadOnlyList<string>>(result.Select(r => r.Name).ToArray());
        }
    }
}
