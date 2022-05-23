using AutoFixture;
using BudgetCast.Common.Application.Behavior.Validation;
using FluentAssertions;
using System;
using System.Collections.Generic;
using BudgetCast.Common.Domain.Results;
using Xunit;

namespace BudgetCast.Common.Application.Tests.Unit.Validation
{
    public class ValidationErrorCodeTests
    {
        private Fixture _fixture;

        public ValidationErrorCodeTests()
        {
            _fixture = new Fixture();
        }

        [Theory]
        [InlineData(ValidationErrorCode.NonExistingDataCode, 1)]
        [InlineData(ValidationErrorCode.BadInputCode, 2)]
        [InlineData(ValidationErrorCode.GeneralErrorCode, 3)]
        public void Ctor_Should_Initialize_Code_And_Severity(string code, int severity)
        {
            // Arrange

            // Act
            var validationErrorCode = new ValidationErrorCode(code, severity);

            // Assert
            validationErrorCode.Code.Should().Be(code);
            validationErrorCode.Severity.Should().Be(severity);
        }

        [Theory]
        [InlineData(ValidationErrorCode.NonExistingDataCode, 1)]
        [InlineData(ValidationErrorCode.BadInputCode, 2)]
        [InlineData(ValidationErrorCode.GeneralErrorCode, 3)]
        public void Parse_Should_Return_Correctly_Initialized_ValidationErrorCode(string code, int severity)
        {
            // Arrange

            // Act
            var validationErrorCode = ValidationErrorCode.Parse(code);

            // Assert
            validationErrorCode.Code.Should().Be(code);
            validationErrorCode.Severity.Should().Be(severity);
        }

        [Theory]
        [InlineData(ValidationErrorCode.NonExistingDataCode, typeof(NotFound))]
        [InlineData(ValidationErrorCode.BadInputCode, typeof(InvalidInput))]
        [InlineData(ValidationErrorCode.GeneralErrorCode, typeof(GeneralFail))]
        public void AsResult_DifferentCode_Values_Should_Return_Correct_Result(string code, Type resultType)
        {
            // Arrange
            var errors = _fixture.Create<Dictionary<string, string[]>>();
            var validationErrorCode = ValidationErrorCode.Parse(code);

            // Act
            var result = validationErrorCode.AsResult(errors);

            // Assert
            result.Should().BeOfType(resultType);
            (result as GeneralFail)!.Errors.Should().BeSameAs(errors);
        }

        [Theory]
        [InlineData(ValidationErrorCode.NonExistingDataCode, typeof(NotFound<int>), typeof(int))]
        [InlineData(ValidationErrorCode.BadInputCode, typeof(InvalidInput<int>), typeof(int))]
        [InlineData(ValidationErrorCode.GeneralErrorCode, typeof(GeneralFail<int>), typeof(int))]
        public void AsGenericResultOf_DifferentCode_Values_Should_Return_Correct_Result(string code, Type resultType, Type underlyingType)
        {
            // Arrange
            var errors = _fixture.Create<Dictionary<string, string[]>>();
            var validationErrorCode = ValidationErrorCode.Parse(code);

            // Act
            var result = validationErrorCode.AsGenericResultOf(underlyingType, errors);

            // Assert
            result.Should().BeOfType(resultType);
            (result as GeneralFail<int>)!.Errors.Should().BeSameAs(errors);
        }
    }
}
