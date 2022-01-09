using FluentValidation;

namespace BudgetCast.Expenses.Queries.Campaigns.GetCampaignTotals
{
    public class GetCampaignTotalsQueryValidator : AbstractValidator<GetCampaignTotalsQuery>
    {
        public GetCampaignTotalsQueryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
