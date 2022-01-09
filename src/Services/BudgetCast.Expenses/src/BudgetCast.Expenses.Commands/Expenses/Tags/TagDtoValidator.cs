using FluentValidation;

namespace BudgetCast.Expenses.Commands.Tags
{
    public class TagDtoValidator : AbstractValidator<TagDto>
    {
        public TagDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
