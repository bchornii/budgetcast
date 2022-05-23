using BudgetCast.Common.Application.Behavior.Validation;
using BudgetCast.Common.Application.Queries;
using BudgetCast.Common.Application.Tests.Unit.Stubs;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Common.Domain.Results;
using Xunit;

namespace BudgetCast.Common.Application.Tests.Unit.Validation
{
    public class QueryValidatorBehaviorTests : ValidationBehaviorTestsBase
    {
        private ValidatorBehaviorFixture<IQuery<Result>, Result> _nonGenericResultBehavior;
        private ValidatorBehaviorFixture<IQuery<Result<FakeData>>, Result<FakeData>> _genericResultBehavior;

        public QueryValidatorBehaviorTests()
        {
            _nonGenericResultBehavior = new ValidatorBehaviorFixture<IQuery<Result>, Result>();
            _genericResultBehavior = new ValidatorBehaviorFixture<IQuery<Result<FakeData>>, Result<FakeData>>();
        }

        #region Verify that generic and non-generic Success result returned when no validation errors found

        [Fact]
        public async Task Handle_NonGenericResultBehavior_NoValidationErrors_Should_Return_Success()
        {
            // Arrange
            var validatorsWithoutErrors = _nonGenericResultBehavior
                .GetValidatorsWithNoErrorResults();
            _nonGenericResultBehavior.AddValidators(validatorsWithoutErrors);

            var successHandler = _nonGenericResultBehavior.HandlerDelegate(new Success());

            // Act
            var result = await _nonGenericResultBehavior
                .Behavior
                .Handle(new FakeQuery(), CancellationToken.None, successHandler);

            // Assert
            result.Should().Be(Success.Empty);
        }

        [Fact]
        public async Task Handle_GenericResultBehavior_NoValidationErrors_Should_Return_Success()
        {
            // Arrange
            var validatorsWithoutErrors = _genericResultBehavior
                .GetValidatorsWithNoErrorResults();
            _genericResultBehavior.AddValidators(validatorsWithoutErrors);

            var fakeData = _genericResultBehavior.FakeData;
            var successHandler = _genericResultBehavior.HandlerDelegate(new Success<FakeData>(fakeData));

            // Act
            var result = await _genericResultBehavior
                .Behavior
                .Handle(new FakeGenericQuery(), CancellationToken.None, successHandler);

            // Assert
            result
                .Should().BeOfType<Success<FakeData>>()
                .Subject.Value
                .Should().Be(fakeData);
        }

        #endregion

        #region Verify command handler result returned as expected

        [Theory]
        [MemberData(nameof(CommonHelpers.GetResultTypes), MemberType = typeof(CommonHelpers))]
        public async Task Handle_NonGenericResultBehavior_NoValidationErrors_Should_Execute_Handler_And_Return_It_Result(Result commandResult)
        {
            // Arrange
            var validatorsWithoutErrors = _nonGenericResultBehavior
                .GetValidatorsWithNoErrorResults();
            _nonGenericResultBehavior.AddValidators(validatorsWithoutErrors);

            var successHandler = _nonGenericResultBehavior.HandlerDelegate(commandResult);

            // Act
            var result = await _nonGenericResultBehavior
                .Behavior
                .Handle(new FakeQuery(), CancellationToken.None, successHandler);

            // Assert
            result.Should().Be(commandResult);
        }

        [Theory]
        [MemberData(nameof(CommonHelpers.GetGenericResultTypes), MemberType = typeof(CommonHelpers))]
        public async Task Handle_GenericResult_Operation_DoesNot_Exist_Should_Execute_Handler_And_Return_It_Result(Result<FakeData> commandResult)
        {
            // Arrange
            var validatorsWithoutErrors = _genericResultBehavior
                .GetValidatorsWithNoErrorResults();
            _genericResultBehavior.AddValidators(validatorsWithoutErrors);

            var handler = _genericResultBehavior.HandlerDelegate(commandResult);

            // Act
            var result = await _genericResultBehavior
                .Behavior
                .Handle(new FakeGenericQuery(), CancellationToken.None, handler);

            // Assert
            result.Should().Be(commandResult);
        }

        #endregion

        #region Verify that for specific error code appropriate Result type is selected

        [Theory]
        [MemberData(nameof(GetErrorCodeWithMappedNonGenericResultType), MemberType = typeof(ValidationBehaviorTestsBase))]
        public async Task Handle_NonGenericResultBehavior_HasValidationError_Should_Return_Expected_ResultType(
            string errorCode,
            Type resultType)
        {
            // Arrange
            var validator = _nonGenericResultBehavior.GetValidator(errorCode);
            _nonGenericResultBehavior.AddValidator(validator);

            var successHandler = _nonGenericResultBehavior.HandlerDelegate(new Success());

            // Act
            var result = await _nonGenericResultBehavior
                .Behavior
                .Handle(new FakeQuery(), CancellationToken.None, successHandler);

            // Assert
            result.Should().BeOfType(resultType);
        }

        [Theory]
        [MemberData(nameof(GetErrorCodeWithMappedGenericResultType), MemberType = typeof(ValidationBehaviorTestsBase))]
        public async Task Handle_GenericResultBehavior_HasValidationError_Should_Return_Expected_ResultType(
            string errorCode,
            Type resultType)
        {
            // Arrange
            var validator = _genericResultBehavior.GetValidator(errorCode);
            _genericResultBehavior.AddValidator(validator);

            var fakeData = _genericResultBehavior.FakeData;
            var successHandler = _genericResultBehavior.HandlerDelegate(new Success<FakeData>(fakeData));

            // Act
            var result = await _genericResultBehavior
                .Behavior
                .Handle(new FakeGenericQuery(), CancellationToken.None, successHandler);

            // Assert
            result.Should().BeOfType(resultType);
        }

        #endregion

        #region Verify that collection of errors generated by Validator(s) returned in Result

        [Fact]
        public async Task Handle_NonGenericResultBehavior_GeneralValidationError_Should_Return_GeneralResult_With_Errors()
        {
            // Arrange
            var validationFailures = _nonGenericResultBehavior
                .GetValidationFailures(ValidationErrorCode.GeneralErrorCode, 5);
            var validator = _nonGenericResultBehavior.GetValidator(validationFailures);
            _nonGenericResultBehavior.AddValidator(validator);

            var errors = validationFailures.GetErrors();

            var successHandler = _nonGenericResultBehavior.HandlerDelegate(new Success());

            // Act
            var result = await _nonGenericResultBehavior
                .Behavior
                .Handle(new FakeQuery(), CancellationToken.None, successHandler);

            // Assert
            result
                .Should().BeOfType<GeneralFail>()
                .Subject.Errors
                .Should().BeEquivalentTo(errors);
        }

        [Fact]
        public async Task Handle_GenericResultBehavior_GeneralValidationError_Should_Return_GeneralResult_With_Errors()
        {
            // Arrange
            var validationFailures = _genericResultBehavior
                .GetValidationFailures(ValidationErrorCode.GeneralErrorCode, 5);
            var validator = _genericResultBehavior.GetValidator(validationFailures);
            _genericResultBehavior.AddValidator(validator);

            var errors = validationFailures.GetErrors();

            var successHandler = _genericResultBehavior.HandlerDelegate(new Success());

            // Act
            var result = await _genericResultBehavior
                .Behavior
                .Handle(new FakeGenericQuery(), CancellationToken.None, successHandler);

            // Assert
            result
                .Should().BeOfType<GeneralFail<FakeData>>()
                .Subject.Errors
                .Should().BeEquivalentTo(errors);
        }

        #endregion

        #region Verify that behavior does not swallow exceptions throwed by command handler

        [Fact]
        public async Task Handle_NonGenericResultBehavior_CommandHandler_Throws_Should_Propagate_Exception()
        {
            // Arrange
            var successHandler = _nonGenericResultBehavior.ExceptionHandlerDelegate();

            // Act
            var result = _nonGenericResultBehavior
                .Behavior
                .Handle(new FakeQuery(), CancellationToken.None, successHandler);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await result);
        }

        [Fact]
        public async Task Handle_GenericResultBehavior_CommandHandler_Throws_Should_Propagate_Exception()
        {
            // Arrange
            var successHandler = _genericResultBehavior.ExceptionHandlerDelegate();

            // Act
            var result = _genericResultBehavior
                .Behavior
                .Handle(new FakeGenericQuery(), CancellationToken.None, successHandler);

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await result);
        }

        #endregion
    }
}
