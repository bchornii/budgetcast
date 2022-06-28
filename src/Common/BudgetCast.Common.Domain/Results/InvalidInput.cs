namespace BudgetCast.Common.Domain.Results;

public record InvalidInput : GeneralFail
{
    public static implicit operator InvalidInput(ValidationError error) => InvalidInput(error);
}

public record InvalidInput<T> : GeneralFail<T>
    where T : notnull
{
    public static implicit operator InvalidInput<T>(ValidationError error) => InvalidInput<T>(error);
}