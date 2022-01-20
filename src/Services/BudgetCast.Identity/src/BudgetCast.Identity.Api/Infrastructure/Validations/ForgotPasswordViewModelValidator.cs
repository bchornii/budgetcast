using BudgetCast.Identity.Api.ViewModels.Account;
using FluentValidation;

namespace BudgetCast.Identity.Api.Infrastructure.Validations
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
