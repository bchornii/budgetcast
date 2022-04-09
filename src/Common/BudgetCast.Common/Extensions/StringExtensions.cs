namespace BudgetCast.Common.Extensions;

public static class StringExtensions
{
    public static long? ToNullableLong(this string value)
        => long.TryParse(value, out var result) ? result : null;

    public static string ToEmptyIfNull(this string? value)
        => !string.IsNullOrWhiteSpace(value) ? value : string.Empty;
}