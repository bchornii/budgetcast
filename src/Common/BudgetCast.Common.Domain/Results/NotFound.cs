namespace BudgetCast.Common.Domain.Results;

public record NotFound : GeneralFail
{
    public static implicit operator NotFound(ValidationError error) => Result.NotFound(error);
}

public record NotFound<T> : GeneralFail<T>
    where T : notnull
{
    public static implicit operator NotFound<T>(ValidationError error) => Result.NotFound<T>(error);
}