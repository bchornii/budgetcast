namespace BudgetCast.Common.Domain.Results;

public record Forbidden : GeneralFail
{
    public static implicit operator Forbidden(ValidationError error) => Forbidden(error);
}

public record Forbidden<T> : GeneralFail<T>
    where T : notnull
{
    public static implicit operator Forbidden<T>(ValidationError error) => Forbidden<T>(error);
}