using BudgetCast.Dashboard.Api.ViewModels;
using BudgetCast.Dashboard.Api.ViewModels.Account;
using FluentValidation;

namespace BudgetCast.Dashboard.Api.Infrastructure.Validations
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
