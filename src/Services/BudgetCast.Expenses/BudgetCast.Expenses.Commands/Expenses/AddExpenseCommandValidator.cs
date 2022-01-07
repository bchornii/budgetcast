using BudgetCast.Expenses.Commands.Tags;
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
                    .SetValidator(new TagDtoValidator());
            });
        }
    }
}
