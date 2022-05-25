namespace BudgetCast.Common.Domain.Results;

public record Success : Result
{
    public static Success Empty { get; } = new();
}

public record Success<T> : Result<T> 
    where T : notnull
{
    public Success(T value)
    {
        Value = value;
    }

    public static implicit operator Success<T>(T value) => new(value);
}