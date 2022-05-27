namespace BudgetCast.Common.Domain.Results;

/// <summary>
/// This type represents success result with no value.
/// </summary>
public record Success : Result
{
    public static Success Empty { get; } = new();
}

/// <summary>
/// This type represents success result with not null value.
/// </summary>
/// <typeparam name="T">Not nullable type</typeparam>
public sealed record Success<T> : Result<T> 
    where T : notnull
{
    public static Success<bool> Empty { get; } = new(true);
    
    public Success(T value)
    {
        Value = value;
    }

    public static implicit operator Success<T>(T value) => new(value);
}