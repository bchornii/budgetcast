using FluentValidation;

namespace BudgetCast.Expenses.Queries.Expenses.GetExpensesForCampaign
{
    public class GetExpensesForCampaignQueryValidator : AbstractValidator<GetExpensesForCampaignQuery>
    {
        public GetExpensesForCampaignQueryValidator()
        {
            RuleFor(x => x.CampaignName)
                .NotEmpty();

            RuleFor(x => x.Page)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);
        }
    }
}
