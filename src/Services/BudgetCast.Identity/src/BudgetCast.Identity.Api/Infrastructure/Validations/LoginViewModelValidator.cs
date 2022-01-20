using BudgetCast.Identity.Api.ViewModels.Account;
using FluentValidation;

namespace BudgetCast.Identity.Api.Infrastructure.Validations
{
    public class LoginViewModelValidator :
        AbstractValidator<LoginViewModel>
    {
        public LoginViewModelValidator()
        {
            RuleFor(m => m.Email)
                .NotEmpty();

            RuleFor(m => m.Password)
                .NotEmpty();
        }
    }
}
