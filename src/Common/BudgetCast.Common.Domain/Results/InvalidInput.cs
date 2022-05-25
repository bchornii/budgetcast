namespace BudgetCast.Common.Domain.Results;

public record InvalidInput : GeneralFail
{
    public static implicit operator InvalidInput(Error error) => InvalidInput(error);
}

public record InvalidInput<T> : GeneralFail<T>
    where T : notnull
{
    public static implicit operator InvalidInput<T>(Error error) => InvalidInput<T>(error);
}