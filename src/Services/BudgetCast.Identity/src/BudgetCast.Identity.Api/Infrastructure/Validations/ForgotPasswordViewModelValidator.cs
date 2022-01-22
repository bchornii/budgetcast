using BudgetCast.Identity.Api.ApiModels.Account;
using FluentValidation;

namespace BudgetCast.Identity.Api.Infrastructure.Validations
{
    public class ForgotPasswordViewModelValidator :
        AbstractValidator<ForgotPasswordVm>
    {
        public ForgotPasswordViewModelValidator()
        {
            RuleFor(m => m.Email)
                .NotEmpty();
        }
    }
}
