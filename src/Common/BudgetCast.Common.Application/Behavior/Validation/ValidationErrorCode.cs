using BudgetCast.Common.Domain.Results;
using BudgetCast.Common.Extensions;

namespace BudgetCast.Common.Application.Behavior.Validation
{
    /// <summary>
    /// Each fluent validation rule can have error code attached in validator via .WithErrorCode method call.
    /// In order to standardize error codes and make automatic selection of error type result derived from <see cref="Result"/>
    /// for error code, <see cref="ValidationErrorCode"/> type is used. Available error code constants are <see cref="GeneralErrorCode"/>,
    /// <see cref="NonExistingDataCode"/> and <see cref="BadInputCode"/>.
    /// If fluent validation rule does not specify error code or error code specified cannot be mapped/parsed by <see cref="ValidationErrorCode"/>
    /// type, then <see cref="GeneralErrorCode"/> code is used by default.
    /// </summary>
    public class ValidationErrorCode
    {
        public const string NonExistingDataCode = "nonexistingdata";
        public const string BadInputCode = "badinput";
        public const string GeneralErrorCode = "generalerror";

        public static readonly ValidationErrorCode NonExistingData = new(NonExistingDataCode, 1);
        public static readonly ValidationErrorCode BadInput = new(BadInputCode, 2);
        public static readonly ValidationErrorCode GeneralError = new(GeneralErrorCode, 3);

        public string Code { get; }

        public int Severity { get; }

        public ValidationErrorCode(string code, int severity)
        {
            Code = code;
            Severity = severity;
        }

        /// <summary>
        /// Parses error code specified for fluent validation rule.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static ValidationErrorCode Parse(string errorCode)
        {
            var errorCodeLowerCase = errorCode?.ToLowerInvariant();
            return errorCodeLowerCase switch
            {
                NonExistingDataCode => NonExistingData,
                BadInputCode => BadInput,
                GeneralErrorCode => GeneralError,
                _ => GeneralError,
            };
        }

        /// <summary>
        /// Based on <see cref="Code"/> selects appropriate untyped error result type.
        /// </summary>
        /// <param name="errors">list of errors associated with result</param>
        /// <returns></returns>
        public Result AsResult(IDictionary<string, List<string>> errors)
        {
            var codeIdLowerCase = Code.ToLowerInvariant();
            return codeIdLowerCase switch
            {
                NonExistingDataCode => new NotFound
                {
                    Errors = errors,
                },
                BadInputCode => new InvalidInput
                {
                    Errors = errors,
                },
                GeneralErrorCode => new GeneralFail
                {
                    Errors = errors,
                },
                _ => new GeneralFail
                {
                    Errors = errors,
                },
            };
        }

        /// <summary>
        /// Based on <see cref="Code"/> selects appropriate typed error result type.
        /// </summary>
        /// <param name="underlyingType">underlying generic result type</param>
        /// <param name="errors">list of errors associated with result</param>
        /// <returns></returns>
        public Result AsGenericResultOf(Type underlyingType, IDictionary<string, List<string>> errors)
        {
            var code = Code.ToLowerInvariant();
            return code switch
            {
                NonExistingDataCode => typeof(NotFound<>)
                    .CreateInstanceOf(underlyingType)
                    .WithErrors(errors),

                BadInputCode => typeof(InvalidInput<>)
                    .CreateInstanceOf(underlyingType)
                    .WithErrors(errors),

                GeneralErrorCode => typeof(GeneralFail<>)
                    .CreateInstanceOf(underlyingType)
                    .WithErrors(errors),

                _ => typeof(GeneralFail<>)
                    .CreateInstanceOf(underlyingType)
                    .WithErrors(errors),
            };
        }
    }
}
