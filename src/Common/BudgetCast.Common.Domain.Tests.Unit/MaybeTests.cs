using BudgetCast.Common.Domain.Results;
using FluentAssertions;
using Xunit;

namespace BudgetCast.Common.Domain.Tests.Unit;

public class MaybeTests
{
    [Fact]
    public void HasValue_Value_Is_Not_Null_Should_Return_True()
    {
        // Arrange
        
        // Act
        var result = new Maybe<FakeData>(new FakeData());

        // Assert
        result.HasValue.Should().BeTrue();
        result.NoValue.Should().BeFalse();
    }
    
    [Fact]
    public void HasValue_Value_Is_Null_Should_Return_False()
    {
        // Arrange
        
        // Act
        var result = new Maybe<FakeData>(null!);

        // Assert
        result.HasValue.Should().BeFalse();
        result.NoValue.Should().BeTrue();
    }

    [Fact]
    public void Implicit_Conversion_From_GenericType_Argument_Should_Create_New_Instance_Of_Typed_Maybe_Type()
    {
        // Arrange
        static Maybe<FakeData> FromData(FakeData data)
            => data;

        var data = new FakeData();
        
        // Act
        var result = FromData(data);

        // Assert
        result.Should().BeOfType<Maybe<FakeData>>();
        result.Value.Should().Be(data);
    }
    
    [Fact]
    public void Implicit_Conversion_From_GenericType_Argument_Of_Null_Should_Create_New_Instance_Of_Typed_Maybe_Type()
    {
        // Arrange
        static Maybe<FakeData> FromData(FakeData data)
            => data;

        // Act
        var result = FromData(null!);

        // Assert
        result.Should().BeOfType<Maybe<FakeData>>();
        result.Value.Should().BeNull();
    }

    [Fact]
    public void FakeMaybe_Should_Initialize_Value()
    {
        // Arrange
        
        // Act
        var result = new FakeMaybe(new FakeData());

        // Assert
        result.Value.Should().BeOfType<FakeData>();
    }

    private sealed record FakeMaybe : Maybe<FakeData>
    {
        public FakeMaybe(FakeData value) : base(value)
        {
            Value = new FakeData();
        }
    }
}