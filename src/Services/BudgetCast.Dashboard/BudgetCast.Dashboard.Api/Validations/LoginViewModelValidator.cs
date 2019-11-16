using BudgetCast.Dashboard.Api.ViewModels;
using FluentValidation;

namespace BudgetCast.Dashboard.Api.Validations
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
