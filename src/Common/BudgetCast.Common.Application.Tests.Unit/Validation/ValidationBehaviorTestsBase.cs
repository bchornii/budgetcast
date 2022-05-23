using AutoFixture;
using BudgetCast.Common.Application.Behavior.Validation;
using BudgetCast.Common.Application.Tests.Unit.Stubs;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Common.Domain.Results;

namespace BudgetCast.Common.Application.Tests.Unit.Validation
{
    public class ValidationBehaviorTestsBase
    {
        protected class ValidatorBehaviorFixture<TRequest, TResponse>
            where TResponse : Result
        {
            private readonly Fixture _fixture;

            public FakeData FakeData { get; }

            public List<IValidator<TRequest>> Validators { get; }

            public ILogger<ValidatorBehavior<TRequest, TResponse>> Logger { get; }

            public ValidatorBehavior<TRequest, TResponse> Behavior { get; }

            public ValidatorBehaviorFixture()
            {
                _fixture = new Fixture();
                FakeData = _fixture.Create<FakeData>();
                Logger = Mock.Of<ILogger<ValidatorBehavior<TRequest, TResponse>>>();
                Validators = new List<IValidator<TRequest>>();
                Behavior = new ValidatorBehavior<TRequest, TResponse>(Validators, Logger);
            }

            /// <summary>
            /// Adds validator to collection of validators used by behavior.
            /// </summary>
            /// <param name="validator"></param>
            public void AddValidator(IValidator<TRequest> validator)
            {
                Validators.Add(validator);
            }

            /// <summary>
            /// Adds validators to collection of validators used by behavior.
            /// </summary>
            /// <param name="validators"></param>
            public void AddValidators(IEnumerable<IValidator<TRequest>> validators)
            {
                Validators.AddRange(validators);
            }

            /// <summary>
            /// Used to represent commands of non-generic response types such as <see cref="ICommand{TResult}"/>
            /// where <c>TResult</c> is <see cref="Result"/>.
            /// </summary>
            public RequestHandlerDelegate<TResponse> HandlerDelegate(Result result)
                => () => Task.FromResult(result as TResponse);

            /// <summary>
            /// Used to represent commands of non-generic response types such as <see cref="ICommand{TResult}"/>
            /// where <c>TResult</c> is <see cref="Result{T}"/>.
            /// </summary>
            public RequestHandlerDelegate<TResponse> HandlerDelegate(Result<FakeData> result)
                => () => Task.FromResult(result as TResponse);

            /// <summary>
            /// Used to represent command handler which throws an exception.
            /// </summary>
            public RequestHandlerDelegate<TResponse> ExceptionHandlerDelegate()
                => () => throw new InvalidOperationException();

            public IValidator<TRequest>[] GetValidatorsWithNoErrorResults()
            {
                var validatorWithoutErrors = GetValidator();
                return Enumerable.Repeat(validatorWithoutErrors, 5).ToArray();
            }

            public IValidator<TRequest> GetValidator()
            {
                var validator = Mock.Of<IValidator<TRequest>>();
                Mock.Get(validator)
                    .Setup(s => s.ValidateAsync(It.IsAny<TRequest>(), CancellationToken.None))
                    .ReturnsAsync(new ValidationResult(Array.Empty<ValidationFailure>()));

                return validator;
            }

            public IValidator<TRequest> GetValidator(string errorCode)
            {
                var validator = Mock.Of<IValidator<TRequest>>();
                var validationFailure = GetValidationFailure(errorCode);
                Mock.Get(validator)
                    .Setup(s => s.ValidateAsync(It.IsAny<TRequest>(), CancellationToken.None))
                    .ReturnsAsync(new ValidationResult(new[] { validationFailure, }));

                return validator;
            }

            public IValidator<TRequest> GetValidator(IEnumerable<ValidationFailure> validationFailures)
            {
                var validator = Mock.Of<IValidator<TRequest>>();
                Mock.Get(validator)
                    .Setup(s => s.ValidateAsync(It.IsAny<TRequest>(), CancellationToken.None))
                    .ReturnsAsync(new ValidationResult(validationFailures));

                return validator;
            }

            public ValidationFailure GetValidationFailure(string errorCode)
                => new(_fixture.Create<string>(), _fixture.Create<string>())
                {
                    ErrorCode = errorCode,
                };

            public IReadOnlyCollection<ValidationFailure> GetValidationFailures(string errorCode, int total)
                => Enumerable.Range(1, total)
                    .Select(_ => new ValidationFailure(_fixture.Create<string>(), _fixture.Create<string>())
                    {
                        ErrorCode = errorCode,
                    }).ToArray();
        }

        public static IEnumerable<object[]> GetErrorCodeWithMappedGenericResultType()
        {
            yield return new object[]
            {
                ValidationErrorCode.GeneralErrorCode,
                typeof(GeneralFail<FakeData>),
            };

            yield return new object[]
            {
                ValidationErrorCode.NonExistingDataCode,
                typeof(NotFound<FakeData>),
            };

            yield return new object[]
            {
                ValidationErrorCode.BadInputCode,
                typeof(InvalidInput<FakeData>),
            };
        }

        public static IEnumerable<object[]> GetErrorCodeWithMappedNonGenericResultType()
        {
            yield return new object[]
            {
                ValidationErrorCode.GeneralErrorCode,
                typeof(GeneralFail),
            };

            yield return new object[]
            {
                ValidationErrorCode.NonExistingDataCode,
                typeof(NotFound),
            };

            yield return new object[]
            {
                ValidationErrorCode.BadInputCode,
                typeof(InvalidInput),
            };
        }
    }
}
