using BudgetCast.Common.Application;
using BudgetCast.Common.Domain.Results;
using BudgetCast.Expenses.Domain.Expenses;
using FluentValidation;

namespace BudgetCast.Expenses.Commands.Expenses
{
    public class AddExpenseCommandValidator : AbstractValidator<AddExpenseCommand>
    {
        public AddExpenseCommandValidator()
        {
            RuleFor(x => x.AddedAt)
                .NotEmpty()
                .WithErrorCode("f");

            RuleFor(x => x.CampaignName)
                .NotEmpty();

            When(x => x.Tags.Any(), () =>
            {
                RuleForEach(x => x.Tags)
                    .MustBeValueObject(value => new Success<Tag>(new Tag{Name = value}))
                    .NotEmpty();
            });
        }
    }

    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeValueObject<T, TValueObject>(
            this IRuleBuilder<T, string> ruleBuilder,
            Func<string, Result<TValueObject>> factory)
        {
            return (IRuleBuilderOptions<T, string>) ruleBuilder.Custom((value, context) =>
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                var result = factory(value);

                if (!result.IsSuccess())
                {
                    context.AddFailure("failure");
                }
            });
        } 
    }
}
