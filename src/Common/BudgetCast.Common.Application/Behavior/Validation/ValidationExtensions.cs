using System.Diagnostics.CodeAnalysis;
using BudgetCast.Common.Domain;
using BudgetCast.Common.Domain.Results;
using BudgetCast.Common.Extensions;
using FluentValidation;
using FluentValidation.Results;

namespace BudgetCast.Common.Application.Behavior.Validation;

[ExcludeFromCodeCoverage]
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

    public static IRuleBuilderOptions<T, string> MustBeEntity<T, TEntity>(
        this IRuleBuilder<T, string> ruleBuilder,
        Func<string, Result<TEntity>> factory)
        where TEntity : Entity
    {
        return (IRuleBuilderOptions<T, string>) ruleBuilder.Custom((value, context) =>
        {
            Result<TEntity> result = factory(value);

            if (!result)
            {
                result
                    .Errors
                    .MapToValidationFailures(context.PropertyName)
                    .ForEach(context.AddFailure);
            }
        });
    }
    
    public static IRuleBuilderOptions<T, DateTime> MustBeValidDate<T>(
        this IRuleBuilder<T, DateTime> ruleBuilder,
        Func<DateTime, Result<DateTime>> factory)
    {
        return (IRuleBuilderOptions<T, DateTime>) ruleBuilder.Custom((value, context) =>
        {
            Result<DateTime> result = factory(value);

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