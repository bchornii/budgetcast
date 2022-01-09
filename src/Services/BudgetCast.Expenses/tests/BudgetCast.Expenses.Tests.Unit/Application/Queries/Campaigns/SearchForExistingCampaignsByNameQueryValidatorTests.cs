using AutoFixture;
using BudgetCast.Expenses.Queries.Campaigns.SearchForExistingCampaignsByName;
using FluentValidation.TestHelper;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Application.Queries.Campaigns
{
    public class SearchForExistingCampaignsByNameQueryValidatorTests
    {
        private SearchForExistingCampaignsByNameQueryValidatorFixture _fixture;

        public SearchForExistingCampaignsByNameQueryValidatorTests()
        {
            _fixture = new SearchForExistingCampaignsByNameQueryValidatorFixture();
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

        private class SearchForExistingCampaignsByNameQueryValidatorFixture
        {
            public SearchForExistingCampaignsByNameQueryValidator Validator { get; }

            public SearchForExistingCampaignsByNameQueryValidatorFixture()
            {
                Validator = new SearchForExistingCampaignsByNameQueryValidator();
            }

            public SearchForExistingCampaignsByNameQuery CreatePopulated()
                => new Fixture().Create<SearchForExistingCampaignsByNameQuery>();
        }
    }
}
