namespace BudgetCast.Common.Domain.Results;

public record NotFound : GeneralFail
{
    public static implicit operator NotFound(Error error) => Result.NotFound(error);
}

public record NotFound<T> : GeneralFail<T>
    where T : notnull
{
    public static implicit operator NotFound<T>(Error error) => Result.NotFound<T>(error);
}