namespace BudgetCast.Gateways.Bff.Extensions;

public static class StringExtensions
{
    public static bool IsMissing(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }
        
    public static bool IsPresent(this string value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
}