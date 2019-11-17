using BudgetCast.Dashboard.Api.ViewModels;
using FluentValidation;

namespace BudgetCast.Dashboard.Api.Validations
{
    public class ForgotPasswordViewModelValidator :
        AbstractValidator<ForgotPasswordViewModel>
    {
        public ForgotPasswordViewModelValidator()
        {
            RuleFor(m => m.Email)
                .NotEmpty();
        }
    }
}
