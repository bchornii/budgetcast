using BudgetCast.Common.Application.Behavior.Validation;
using BudgetCast.Expenses.Domain.Campaigns;
using FluentValidation;

namespace BudgetCast.Expenses.Commands.Campaigns.CreateMonthlyCampaign
{
    public class CreateMonthlyCampaignCommandValidator : 
        AbstractValidator<CreateMonthlyCampaignCommand>
    {
        public CreateMonthlyCampaignCommandValidator(ICampaignRepository campaignRepository)
        {
            RuleFor(x => x.Name)
                .MustBeEntity(Campaign.Create);
        }
    }
}
