using BudgetCast.Common.Domain;
using BudgetCast.Common.Domain.Results;
using BudgetCast.Common.Extensions;
using FluentValidation;
using FluentValidation.Results;

namespace BudgetCast.Common.Application.Behavior.Validation;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeValueObject<T, TValueObject>(
        this IRuleBuilder<T, string> ruleBuilder,
        Func<string, Result<TValueObject>> factory) 
        where TValueObject : ValueObject
    {
        return (IRuleBuilderOptions<T, string>) ruleBuilder.Custom((value, context) =>
        {
            Result<TValueObject> result = factory(value);

            if (!result)
            {
                result
                    .Errors
                    .MapToValidationFailures(context.PropertyName)
                    .ForEach(context.AddFailure);
            }
        });
    }

    private static IEnumerable<ValidationFailure> MapToValidationFailures(
        this IDictionary<string, List<string>> errors, 
        string propertyName)
    {
        return errors
            .SelectMany(kv => kv.Value
                .Select(v => new ValidationFailure(propertyName, v)
                {
                    ErrorCode = kv.Key,
                    Severity = Severity.Error,
                }));
    }
}