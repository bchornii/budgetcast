using BudgetCast.Expenses.Queries.Campaigns.GetCampaignTotals;
using FluentValidation.TestHelper;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Application.Queries.Campaigns
{
    public class GetCampaignTotalsQueryValidatorTests
    {
        private GetCampaignTotalsQueryValidator _validator;

        public GetCampaignTotalsQueryValidatorTests()
        {
            _validator = new GetCampaignTotalsQueryValidator();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Name_HasDefaultValue_IsInvalid(string name)
        {
            // Arrange

            // Act
            var result = _validator.TestValidate(new GetCampaignTotalsQuery(name));

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }
    }
}
