using BudgetCast.Dashboard.Api.ViewModels;
using FluentValidation;

namespace BudgetCast.Dashboard.Api.Validations
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
