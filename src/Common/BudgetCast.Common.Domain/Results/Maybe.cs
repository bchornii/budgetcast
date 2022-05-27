namespace BudgetCast.Common.Domain.Results;

public record Maybe<T>(T Value) : Result<T>
{
    /// <summary>
    /// Returns <c>true</c> if <see cref="Value"/> contains not nullable value.
    /// </summary>
    public bool HasValue => Value is not null;

    /// <summary>
    /// Returns <c>true</c> if <see cref="Value"/> is <c>null</c>;
    /// </summary>
    public bool NoValue => !HasValue;
    
    /// <summary>
    /// Operation result type
    /// </summary>
    public override T Value { get; protected init; } = Value;

    /// <summary>
    /// Converts instances of <typeparamref name="T"/> into <see cref="Maybe{T}"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator Maybe<T>(T value) => new(value);
}