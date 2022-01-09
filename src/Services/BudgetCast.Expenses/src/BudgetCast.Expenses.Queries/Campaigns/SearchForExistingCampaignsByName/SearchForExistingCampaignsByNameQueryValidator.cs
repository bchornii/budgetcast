using FluentValidation;

namespace BudgetCast.Expenses.Queries.Campaigns.SearchForExistingCampaignsByName
{
    public class SearchForExistingCampaignsByNameQueryValidator : 
        AbstractValidator<SearchForExistingCampaignsByNameQuery>
    {
        public SearchForExistingCampaignsByNameQueryValidator()
        {
            RuleFor(x => x.Term)
                .NotEmpty();

            RuleFor(x => x.Amount)
                .InclusiveBetween(1, 100);
        }
    }
}
