using BudgetCast.Expenses.Domain.Campaigns;
using FluentValidation;

namespace BudgetCast.Expenses.Commands.Campaigns.CreateMonthlyCampaign;

public class CreateMonthlyCampaignCommandValidator : 
    AbstractValidator<CreateMonthlyCampaignCommand>
{
    public CreateMonthlyCampaignCommandValidator(ICampaignRepository campaignRepository)
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MustAsync(async (name, cancellationToken) =>
            {
                var exists = await campaignRepository.ExistsAsync(name, cancellationToken);
                return !exists;
            }).WithMessage("Campaign name must be unique");
    }
}