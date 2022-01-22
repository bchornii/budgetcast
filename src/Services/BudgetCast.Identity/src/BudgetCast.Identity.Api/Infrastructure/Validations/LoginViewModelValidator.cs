using BudgetCast.Identity.Api.ApiModels.Account;
using FluentValidation;

namespace BudgetCast.Identity.Api.Infrastructure.Validations
{
    public class LoginViewModelValidator :
        AbstractValidator<LoginVm>
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
