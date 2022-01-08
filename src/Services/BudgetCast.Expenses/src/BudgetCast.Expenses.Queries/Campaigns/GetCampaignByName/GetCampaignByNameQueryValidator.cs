using FluentValidation;

namespace BudgetCast.Expenses.Queries.Campaigns.GetCampaignByName
{
    public class GetCampaignByNameQueryValidator : AbstractValidator<GetCampaignByNameQuery>
    {
        public GetCampaignByNameQueryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
