using FluentValidation;

namespace BudgetCast.Expenses.Commands.Tags
{
    public class UpdateExpenseTagsCommandValidator 
        : AbstractValidator<UpdateExpenseTagsCommand>
    {
        public UpdateExpenseTagsCommandValidator()
        {
            RuleFor(x => x.ExpenseId)
                .NotEmpty();

            RuleFor(x => x.Tags)
                .NotEmpty();

            RuleForEach(x => x.Tags)
                .SetValidator(new TagDtoValidator());
        }
    }
}
