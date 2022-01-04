using FluentValidation;

namespace BudgetCast.Expenses.Commands.Campaigns
{
    public class CreateMonthlyCampaignCommandValidator : 
        AbstractValidator<CreateMonthlyCampaignCommand>
    {
        public CreateMonthlyCampaignCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
