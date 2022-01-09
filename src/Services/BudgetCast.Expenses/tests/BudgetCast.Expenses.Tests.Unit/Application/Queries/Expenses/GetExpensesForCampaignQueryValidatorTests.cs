using AutoFixture;
using BudgetCast.Expenses.Queries.Expenses.GetExpensesForCampaign;
using FluentValidation.TestHelper;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Application.Queries.Expenses
{
    public class GetExpensesForCampaignQueryValidatorTests
    {
        private GetExpensesForCampaignQueryValidatorFixture _fixture;

        public GetExpensesForCampaignQueryValidatorTests()
        {
            _fixture = new GetExpensesForCampaignQueryValidatorFixture();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void CampaignName_HasDefaultValue_IsInvalid(string name)
        {
            // Arrange
            var defaultQuery = _fixture.CreatePopulated();
            var query = defaultQuery with { CampaignName = name };

            // Act
            var result = _fixture.Validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CampaignName);
        }

        [Fact]
        public void PageNumber_Is_0_IsInvalid()
        {
            // Arrange
            var defaultQuery = _fixture.CreatePopulated();
            var query = defaultQuery with { Page = 0 };

            // Act
            var result = _fixture.Validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Page);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public void PageSize_Is_LessThan_1_Or_MoreThan_100_IsInvalid(int pageSize)
        {
            // Arrange
            var defaultQuery = _fixture.CreatePopulated();
            var query = defaultQuery with { PageSize = pageSize };

            // Act
            var result = _fixture.Validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PageSize);
        }

        private class GetExpensesForCampaignQueryValidatorFixture
        {
            public GetExpensesForCampaignQueryValidator Validator { get; }

            public GetExpensesForCampaignQueryValidatorFixture()
            {
                Validator = new GetExpensesForCampaignQueryValidator();
            }

            public GetExpensesForCampaignQuery CreatePopulated()
                => new Fixture().Create<GetExpensesForCampaignQuery>();
        }
    }
}
