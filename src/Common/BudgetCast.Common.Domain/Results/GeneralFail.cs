namespace BudgetCast.Common.Domain.Results;

public record GeneralFail : Result
{
    public static implicit operator GeneralFail(Error error) => GeneralFail(error);
}

public record GeneralFail<T> : Result<T>
    where T : notnull
{
    public static implicit operator GeneralFail<T>(Error error) => GeneralFail<T>(error);
}