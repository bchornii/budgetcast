using FluentValidation.Results;

namespace BudgetCast.Common.Application.Behavior.Validation
{
    public static class ValidationFailureExtensions
    {
        public static Dictionary<string, List<string>> GetErrors(this IEnumerable<ValidationFailure> validationFailures)
        {
            return validationFailures
                .ToLookup(
                    f => string.IsNullOrEmpty(f.ErrorCode) || !f.ErrorCode.Contains('.')
                        ? "app.general"
                        : f.ErrorCode,
                    f => f.ErrorMessage)
                .ToDictionary(e => e.Key, e => e.ToList());
        }

        public static ValidationErrorCode GetMostSevereErrorCode(this IEnumerable<ValidationFailure> validationFailures)
        {
            return validationFailures
                .Select(f => ValidationErrorCode.Parse(f.ErrorCode))
                .OrderBy(code => code.Severity)
                .First();
        }
    }
}
