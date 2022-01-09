namespace BudgetCast.Common.Application.Behavior.Validation
{
    public static class ValidationErrorCodeExtensions
    {
        public static ValidationErrorCode OrGeneralErrorCodeIfNull(this ValidationErrorCode validationErrorCode)
            => validationErrorCode ?? ValidationErrorCode.GeneralError;
    }
}
