using BudgetCast.Common.Domain.Results;
using FluentAssertions;
using Xunit;

namespace BudgetCast.Common.Domain.Tests.Unit;

public class SuccessTests
{
    [Fact]
    public void Empty_Should_Return_The_Same_Instance_Of_Success_When_Called_Multiple_Times()
    {
        // Arrange
        
        // Act
        var result1 = Success.Empty;
        var result2 = Success.Empty;
        var result3 = Success.Empty;

        // Assert
        result1
            .Should().Be(result2)
            .And
            .Subject
            .Should().Be(result3);
    }

    [Fact]
    public void Implicit_Conversion_From_GenericType_Argument_Should_Create_New_Instance_Of_Typed_Success_Type()
    {
        // Arrange
        static Success<FakeData> FromData(FakeData data)
            => data;

        var data = new FakeData();
        
        // Act
        var result = FromData(data);

        // Assert
        result.Should().BeOfType<Success<FakeData>>();
        result.Value.Should().Be(data);
    }
}