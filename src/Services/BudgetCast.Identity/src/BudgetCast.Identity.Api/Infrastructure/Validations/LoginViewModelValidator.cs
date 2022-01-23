using BudgetCast.Identity.Api.ApiModels.SignIn;
using FluentValidation;

namespace BudgetCast.Identity.Api.Infrastructure.Validations
{
    public class LoginViewModelValidator :
        AbstractValidator<LoginDto>
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
