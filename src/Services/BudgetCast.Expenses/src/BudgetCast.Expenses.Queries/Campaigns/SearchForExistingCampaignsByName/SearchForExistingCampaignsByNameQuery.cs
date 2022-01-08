using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Queries;

namespace BudgetCast.Expenses.Queries.Campaigns.SearchForExistingCampaignsByName
{
    public record SearchForExistingCampaignsByNameQuery(int Amount, string NameTerm) : 
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
                .GetAsync(request.Amount, request.NameTerm, cancellationToken);
            return new Success<IReadOnlyList<string>>(result.Select(r => r.Name).ToArray());
        }
    }
}
