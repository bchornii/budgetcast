using BudgetCast.Common.Domain.Results;
using BudgetCast.Common.Extensions;

namespace BudgetCast.Common.Application.Behavior.Validation
{
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

        public static ValidationErrorCode Parse(string errorCode)
        {
            var errorCodeLowerCase = errorCode.ToLowerInvariant();
            return errorCodeLowerCase switch
            {
                NonExistingDataCode => NonExistingData,
                BadInputCode => BadInput,
                GeneralErrorCode => GeneralError,
                _ => GeneralError,
            };
        }

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
