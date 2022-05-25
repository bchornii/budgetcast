using BudgetCast.Common.Application.Behavior.Validation;
using BudgetCast.Expenses.Domain.Campaigns;
using BudgetCast.Expenses.Domain.Expenses;
using FluentValidation;

namespace BudgetCast.Expenses.Commands.Expenses
{
    public class AddExpenseCommandValidator : AbstractValidator<AddExpenseCommand>
    {
        public AddExpenseCommandValidator()
        {
            RuleFor(x => x.AddedAt)
                .MustBeValidDate(Expense.IsValidAddingDt);

            RuleFor(x => x.CampaignName)
                .MustBeEntity(Campaign.Create);

            When(x => x.Tags.Any(), () =>
            {
                RuleForEach(x => x.Tags)
                    .MustBeValueObject(Tag.Create);
            });
        }
    }
}
