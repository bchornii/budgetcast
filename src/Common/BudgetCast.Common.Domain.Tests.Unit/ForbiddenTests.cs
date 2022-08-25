using AutoFixture;
using BudgetCast.Common.Domain.Results;
using FluentAssertions;
using Xunit;

namespace BudgetCast.Common.Domain.Tests.Unit;

public class ForbiddenTests
{
    private readonly ForbiddenFixture _fixture;

    public ForbiddenTests()
    {
        _fixture = new ForbiddenFixture();
    }
    
    [Fact]
    public void Implicit_Conversion_From_ErrorType_Should_Create_New_Instance_Of_Forbidden_Type()
    {
        // Arrange
        static Forbidden FromError(ValidationError error)
            => error;
        
        var error = new ValidationError(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        
        // Act
        var result = FromError(error);

        // Assert
        result.Should().BeOfType<Forbidden>();
        result.Errors[error.Code].Should().Contain(v => v == error.Value);
    }
    
    [Fact]
    public void Implicit_Conversion_From_ErrorType_Should_Create_New_Instance_Of_Generic_Forbidden_Type()
    {
        // Arrange
        static Forbidden<FakeData> FromError(ValidationError error)
            => error;
        
        var error = new ValidationError(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        
        // Act
        var result = FromError(error);

        // Assert
        result.Should().BeOfType<Forbidden<FakeData>>();
        result.Errors[error.Code].Should().Contain(v => v == error.Value);
    }
    
    private sealed class ForbiddenFixture
    {
        public Fixture Fixture { get; }

        public ForbiddenFixture()
        {
            Fixture = new Fixture();
        }
    }
}