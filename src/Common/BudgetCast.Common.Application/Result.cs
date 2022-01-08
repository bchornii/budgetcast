namespace BudgetCast.Common.Application
{
    #region Abstract Result types

    public record Result;

    public record Result<T> : Result
    {
        public static implicit operator Result<T>(T value)
            => value == null ? new NotFound<T>() : new Success<T>(value);
    }

    #endregion

    #region Success Result types

    public record Success : Result
    {
        public static Success Empty { get; } = new();
    }

    public record Success<T>(T Data) : Result<T>
        where T : notnull;

    #endregion

    #region Failure error types

    public record GeneralFail : Result
    {
        public IDictionary<string, string[]> Errors { get; init; } = null!;
    }

    public record InvalidInput : GeneralFail;

    public record NotFound : GeneralFail;

    public record GeneralFail<T> : Result<T>
    {
        public IDictionary<string, string[]> Errors { get; init; } = null!;
    }

    public record InvalidInput<T> : GeneralFail<T>;

    public record NotFound<T> : GeneralFail<T>;

    #endregion
}
