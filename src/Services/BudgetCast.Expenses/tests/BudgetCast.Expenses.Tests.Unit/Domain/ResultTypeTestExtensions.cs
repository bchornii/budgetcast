using System.Reflection;
using BudgetCast.Common.Domain.Results;

namespace BudgetCast.Expenses.Tests.Unit.Domain;

public static class ResultTypeTestExtensions
{
    public static Result<TValue> SetUnderlyingValue<TValue>(this Result<TValue> result)
        where TValue : notnull, new()
    {
        var isValueSetField = result.GetType().BaseType!
            .GetField("_isValueSet", BindingFlags.NonPublic | BindingFlags.Instance)!;
        isValueSetField.SetValue(result, true);

        var valueProperty = result.GetType().BaseType!
            .GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance)!;
        valueProperty.SetValue(result, new TValue());

        return result;
    }
    
    public static GeneralFail<TValue> SetUnderlyingValue<TValue>(this GeneralFail<TValue> result)
        where TValue : notnull, new()
    {
        var isValueSetField = result.GetType().BaseType!
            .GetField("_isValueSet", BindingFlags.NonPublic | BindingFlags.Instance)!;
        isValueSetField.SetValue(result, true);

        var valueProperty = result.GetType().BaseType!
            .GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance)!;
        valueProperty.SetValue(result, new TValue());

        return result;
    }

    public static InvalidInput<TValue> SetUnderlyingValue<TValue>(this InvalidInput<TValue> result)
        where TValue : notnull, new()
    {
        var isValueSetField = result.GetType().BaseType!.BaseType!
            .GetField("_isValueSet", BindingFlags.NonPublic | BindingFlags.Instance)!;
        isValueSetField.SetValue(result, true);

        var valueProperty = result.GetType().BaseType!.BaseType!
            .GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance)!;
        valueProperty.SetValue(result, new TValue());

        return result;
    }

    public static NotFound<TValue> SetUnderlyingValue<TValue>(this NotFound<TValue> result)
        where TValue : notnull, new()
    {
        var isValueSetField = result.GetType().BaseType!.BaseType!
            .GetField("_isValueSet", BindingFlags.NonPublic | BindingFlags.Instance)!;
        isValueSetField.SetValue(result, true);

        var valueProperty = result.GetType().BaseType!.BaseType!
            .GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance)!;
        valueProperty.SetValue(result, new TValue());

        return result;
    }
}