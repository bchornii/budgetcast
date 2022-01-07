﻿using FluentValidation.Results;

namespace BudgetCast.Common.Application.Behavior.Validation
{
    public static class ValidationFailureExtensions
    {
        public static Dictionary<string, string[]> GetErrors(this IEnumerable<ValidationFailure> validationFailures)
        {
            return validationFailures
                .ToLookup(
                    f => string.IsNullOrEmpty(f.PropertyName)
                        ? "generalErrors"
                        : f.PropertyName,
                    f => f.ErrorMessage)
                .ToDictionary(e => e.Key, e => e.ToArray());
        }

        public static ValidationErrorCode GetMostSevereErrorCode(this IEnumerable<ValidationFailure> validationFailures)
        {
            return validationFailures
                .Select(f => ValidationErrorCode.Parse(f.ErrorCode))
                .OrderByDescending(code => code.Severity)
                .First();
        }
    }
}