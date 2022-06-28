using AutoFixture;
using BudgetCast.Common.Domain.Results;
using FluentAssertions;
using Xunit;

namespace BudgetCast.Common.Domain.Tests.Unit;

public class InvalidInputTests
{
    private InvalidInputFixture _fixture;

    public InvalidInputTests()
    {
        _fixture = new InvalidInputFixture();
    }
    
    [Fact]
    public void Implicit_Conversion_From_ErrorType_Should_Create_New_Instance_Of_InvalidInput_Type()
    {
        // Arrange
        static InvalidInput FromError(ValidationError error)
            => error;
        
        var error = new ValidationError(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        
        // Act
        var result = FromError(error);

        // Assert
        result.Should().BeOfType<InvalidInput>();
        result.Errors[error.Code].Should().Contain(v => v == error.Value);
    }
    
    [Fact]
    public void Implicit_Conversion_From_ErrorType_Should_Create_New_Instance_Of_Generic_InvalidInput_Type()
    {
        // Arrange
        static InvalidInput<FakeData> FromError(ValidationError error)
            => error;
        
        var error = new ValidationError(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        
        // Act
        var result = FromError(error);

        // Assert
        result.Should().BeOfType<InvalidInput<FakeData>>();
        result.Errors[error.Code].Should().Contain(v => v == error.Value);
    }
    
    private sealed class InvalidInputFixture
    {
        public Fixture Fixture { get; }

        public InvalidInputFixture()
        {
            Fixture = new Fixture();
        }
    }
}