using BudgetCast.Expenses.Commands.Tags;
using FluentValidation.TestHelper;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Application.Expenses
{
    public class TagDtoValidatorTests
    {
        private TagDtoValidator _tagDtoValidator;

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
}
