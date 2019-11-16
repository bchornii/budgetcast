using BudgetCast.Dashboard.Api.ViewModels;
using FluentValidation;

namespace BudgetCast.Dashboard.Api.Validations
{
    public class RegisterViewModelValidtor :
        AbstractValidator<RegisterViewModel>
    {
        public RegisterViewModelValidtor()
        {
            RuleFor(m => m.Email)
                .NotEmpty();

            RuleFor(m => m.GivenName)
                .NotEmpty();

            RuleFor(m => m.SurName)
                .NotEmpty();

            RuleFor(m => m.Password)
                .NotEmpty();

            RuleFor(m => m.PasswordConfirm)
                .NotEmpty();

            RuleFor(m => m.Password)
                .Equal(m => m.PasswordConfirm)
                .WithMessage("Passwords are not the same");                
        }
    }
}
