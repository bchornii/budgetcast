using AutoFixture;
using BudgetCast.Common.Domain.Results;
using FluentAssertions;
using Xunit;

namespace BudgetCast.Common.Domain.Tests.Unit;

public class GeneralFailTests
{
    private GeneralFailFixture _fixture;

    public GeneralFailTests()
    {
        _fixture = new GeneralFailFixture();
    }
    
    [Fact]
    public void Implicit_Conversion_From_ErrorType_Should_Create_New_Instance_Of_GeneralFail_Type()
    {
        // Arrange
        static GeneralFail FromError(Error error)
            => error;
        
        var error = new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        
        // Act
        var result = FromError(error);

        // Assert
        result.Should().BeOfType<GeneralFail>();
        result.Errors[error.Code].Should().Contain(v => v == error.Value);
    }
    
    [Fact]
    public void Implicit_Conversion_From_ErrorType_Should_Create_New_Instance_Of_Generic_GeneralFail_Type()
    {
        // Arrange
        static GeneralFail<FakeData> FromError(Error error)
            => error;
        
        var error = new Error(_fixture.Fixture.Create<string>(), _fixture.Fixture.Create<string>());
        
        // Act
        var result = FromError(error);

        // Assert
        result.Should().BeOfType<GeneralFail<FakeData>>();
        result.Errors[error.Code].Should().Contain(v => v == error.Value);
    }
    
    private sealed class GeneralFailFixture
    {
        public Fixture Fixture { get; }

        public GeneralFailFixture()
        {
            Fixture = new Fixture();
        }
    }
}