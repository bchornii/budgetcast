using System;
using AutoFixture;
using BudgetCast.Common.Domain.Results;
using BudgetCast.Common.Domain.Results.Exceptions;
using FluentAssertions;
using Xunit;

namespace BudgetCast.Common.Domain.Tests.Unit;

public class ResultGenericTests
{
    private ResultGenericFixture _fixture;

    public ResultGenericTests()
    {
        _fixture = new ResultGenericFixture();
    }
    
    [Fact]
    public void Implicit_Conversion_From_ErrorType_Should_Create_New_Instance_Of_Typed_GenericFail_Type()
    {
        // Arrange
        static Result<FakeData> FromError(Error error)
            => error;

        var error = new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        
        // Act
        var result = FromError(error);

        // Assert
        result.Should().BeOfType<GeneralFail<FakeData>>();
        result.Errors[error.Code].Should().Contain(v => v == error.Value);
    }

    [Fact]
    public void Implicit_Conversion_From_Generic_Type_Argument_Should_Create_New_Instance_Of_Typed_Success_Type()
    {
        // Arrange
        static Result<FakeData> FromData(FakeData data)
            => data;

        var data = new FakeData();
        
        // Act
        var result = FromData(data);

        // Assert
        result.Should().BeOfType<Success<FakeData>>();
        result.Value.Should().Be(data);
    }

    [Fact]
    public void FakeResult_Is_Being_Created_With_Null_Passed_To_Value_Property_Should_Throw_Exception()
    {
        // Arrange

        // Act
        var action = () =>
        {
            _ = new FakeResult(null!);
        };

        // Assert
        Assert.Throws<ResultValueIsNullException>(action);
    }

    [Fact]
    public void FakeResult_Is_Initialized_With_Value_Should_Return_This_Value()
    {
        // Arrange
        var data = new FakeData();

        // Act
        var result = new FakeResult(data);

        // Assert
        result.Value.Should().Be(data);
    }
    
    private sealed class ResultGenericFixture
    {
        public Fixture Fixture { get; }

        public ResultGenericFixture()
        {
            Fixture = new Fixture();
        }
    }

    public record FakeResult : Result<FakeData>
    {
        public FakeResult(FakeData data)
        {
            Value = data;
        }
    }
}