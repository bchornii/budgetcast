namespace BudgetCast.Common.Domain.Results;

public record GeneralFail : Result
{
    public static implicit operator GeneralFail(ValidationError error) => GeneralFail(error);
}

public record GeneralFail<T> : Result<T>
    where T : notnull
{
    public static implicit operator GeneralFail<T>(ValidationError error) => GeneralFail<T>(error);
}