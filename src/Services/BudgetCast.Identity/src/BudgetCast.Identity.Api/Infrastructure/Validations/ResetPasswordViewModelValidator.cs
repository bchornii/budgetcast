using BudgetCast.Identity.Api.ViewModels.Account;
using FluentValidation;

namespace BudgetCast.Identity.Api.Infrastructure.Validations
{
    public class ResetPasswordViewModelValidator :
        AbstractValidator<ResetPasswordViewModel>
    {
        public ResetPasswordViewModelValidator()
        {
            RuleFor(m => m.Email)
                .NotEmpty();

            RuleFor(m => m.Password)
                .NotEmpty();

            RuleFor(m => m.PasswordConfirm)
                .NotEmpty();

            RuleFor(m => m.Code)
                .NotEmpty();

            RuleFor(m => m.Password)
                .Equal(m => m.PasswordConfirm)
                .WithMessage("Passwords are not the same");
        }
    }
}
