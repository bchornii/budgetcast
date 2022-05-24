﻿using BudgetCast.Common.Application.Behavior.Validation;
using BudgetCast.Expenses.Domain.Expenses;
using FluentValidation;

namespace BudgetCast.Expenses.Commands.Expenses
{
    public class AddExpenseCommandValidator : AbstractValidator<AddExpenseCommand>
    {
        public AddExpenseCommandValidator()
        {
            RuleFor(x => x.AddedAt)
                .NotEmpty();

            RuleFor(x => x.CampaignName)
                .NotEmpty();

            When(x => x.Tags.Any(), () =>
            {
                RuleForEach(x => x.Tags)
                    .MustBeValueObject(Tag.Create);
            });
        }
    }
}
