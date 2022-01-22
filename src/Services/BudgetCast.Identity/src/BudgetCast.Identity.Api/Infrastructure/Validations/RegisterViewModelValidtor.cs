using BudgetCast.Identity.Api.ApiModels.Account;
using FluentValidation;

namespace BudgetCast.Identity.Api.Infrastructure.Validations
{
    public class RegisterViewModelValidtor :
        AbstractValidator<RegisterVm>
    {
        public RegisterViewModelValidtor()
        {
            RuleFor(m => m.Email)
                .NotEmpty();

            RuleFor(m => m.FirstName)
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
