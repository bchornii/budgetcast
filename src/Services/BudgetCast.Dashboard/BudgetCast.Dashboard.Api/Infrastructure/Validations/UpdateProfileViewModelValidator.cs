using BudgetCast.Dashboard.Api.ViewModels;
using BudgetCast.Dashboard.Api.ViewModels.Account;
using FluentValidation;

namespace BudgetCast.Dashboard.Api.Infrastructure.Validations
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
