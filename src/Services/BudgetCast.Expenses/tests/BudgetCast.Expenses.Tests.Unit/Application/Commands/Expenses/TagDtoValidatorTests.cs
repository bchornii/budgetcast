using BudgetCast.Expenses.Commands.Expenses.Tags;
using FluentValidation.TestHelper;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Application.Commands.Expenses;

public class TagDtoValidatorTests
{
    private readonly TagDtoValidator _tagDtoValidator;

    public TagDtoValidatorTests()
    {
        _tagDtoValidator = new TagDtoValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Name_IsEmpty_IsInvalid(string name)
    {
        // Arrange

        // Act
        var result = _tagDtoValidator.TestValidate(new TagDto(name));

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}