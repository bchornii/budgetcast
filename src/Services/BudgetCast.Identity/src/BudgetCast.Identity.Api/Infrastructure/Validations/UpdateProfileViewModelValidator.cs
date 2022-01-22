﻿using BudgetCast.Identity.Api.ApiModels.Account;
using FluentValidation;

namespace BudgetCast.Identity.Api.Infrastructure.Validations
{
    public class UpdateProfileViewModelValidator :
        AbstractValidator<UpdateProfileVm>
    {
        public UpdateProfileViewModelValidator()
        {
            RuleFor(m => m.GivenName)
                .NotEmpty();

            RuleFor(m => m.SurName)
                .NotEmpty();
        }
    }
}
