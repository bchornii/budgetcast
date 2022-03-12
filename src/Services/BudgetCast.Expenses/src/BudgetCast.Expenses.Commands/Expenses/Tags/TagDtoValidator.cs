using FluentValidation;

namespace BudgetCast.Expenses.Commands.Expenses.Tags;

public class TagDtoValidator : AbstractValidator<TagDto>
{
    public TagDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}