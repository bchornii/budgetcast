using BudgetCast.Identity.Api.ViewModels.Account;
using FluentValidation;

namespace BudgetCast.Identity.Api.Infrastructure.Validations
{
    public class UpdateProfileViewModelValidator :
        AbstractValidator<UpdateProfileViewModel>
    {
        public UpdateProfileViewModelValidator()
        {
            RuleFor(m => m.GivenName)
                .NotEmpty();

            RuleFor(m => m.SurName)
                .NotEmpty();
        }
    }
}
