using BudgetCast.Dashboard.Api.ViewModels;
using BudgetCast.Dashboard.Api.ViewModels.Account;
using FluentValidation;

namespace BudgetCast.Dashboard.Api.Infrastructure.Validations
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
