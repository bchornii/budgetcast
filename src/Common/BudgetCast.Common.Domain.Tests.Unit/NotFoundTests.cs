using AutoFixture;
using BudgetCast.Common.Domain.Results;
using FluentAssertions;
using Xunit;

namespace BudgetCast.Common.Domain.Tests.Unit;

public class NotFoundTests
{
    private NotFoundFixture _fixture;

    public NotFoundTests()
    {
        _fixture = new NotFoundFixture();
    }
    
    [Fact]
    public void Implicit_Conversion_From_ErrorType_Should_Create_New_Instance_Of_NotFound_Type()
    {
        // Arrange
        static NotFound FromError(ValidationError error)
            => error;
        
        var error = new ValidationError(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        
        // Act
        var result = FromError(error);

        // Assert
        result.Should().BeOfType<NotFound>();
        result.Errors[error.Code].Should().Contain(v => v == error.Value);
    }
    
    [Fact]
    public void Implicit_Conversion_From_ErrorType_Should_Create_New_Instance_Of_Generic_NotFound_Type()
    {
        // Arrange
        static NotFound<FakeData> FromError(ValidationError error)
            => error;
        
        var error = new ValidationError(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        
        // Act
        var result = FromError(error);

        // Assert
        result.Should().BeOfType<NotFound<FakeData>>();
        result.Errors[error.Code].Should().Contain(v => v == error.Value);
    }
    
    private sealed class NotFoundFixture
    {
        public Fixture Fixture { get; }

        public NotFoundFixture()
        {
            Fixture = new Fixture();
        }
    }
}