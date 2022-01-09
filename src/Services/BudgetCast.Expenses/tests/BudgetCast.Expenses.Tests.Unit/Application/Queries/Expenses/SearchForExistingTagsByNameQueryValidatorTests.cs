using AutoFixture;
using BudgetCast.Expenses.Queries.Expenses.SearchForExistingTagsByName;
using FluentValidation.TestHelper;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Application.Queries.Expenses
{
    public class SearchForExistingTagsByNameQueryValidatorTests
    {
        private SearchForExistingTagsByNameQueryValidatorTestsFixture _fixture;

        public SearchForExistingTagsByNameQueryValidatorTests()
        {
            _fixture = new SearchForExistingTagsByNameQueryValidatorTestsFixture();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void NameTerm_HasDefaultValue_IsInvalid(string term)
        {
            // Arrange
            var defaultQuery = _fixture.CreatePopulated();
            var query = defaultQuery with { Term = term };

            // Act
            var result = _fixture.Validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Term);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public void Amount_IsLessThan_1_Or_MoreThan_100_IsInvalid(int amount)
        {
            // Arrange
            var defaultQuery = _fixture.CreatePopulated();
            var query = defaultQuery with { Amount = amount };

            // Act
            var result = _fixture.Validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Amount);
        }

        private class SearchForExistingTagsByNameQueryValidatorTestsFixture
        {
            public SearchForExistingTagsByNameQueryValidator Validator { get; }

            public SearchForExistingTagsByNameQueryValidatorTestsFixture()
            {
                Validator = new SearchForExistingTagsByNameQueryValidator();
            }

            public SearchForExistingTagsByNameQuery CreatePopulated()
                => new Fixture().Create<SearchForExistingTagsByNameQuery>();
        }
    }
}
