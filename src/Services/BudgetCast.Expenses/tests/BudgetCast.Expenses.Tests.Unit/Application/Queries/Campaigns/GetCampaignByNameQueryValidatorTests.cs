using BudgetCast.Expenses.Queries.Campaigns.GetCampaignByName;
using FluentValidation.TestHelper;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Application.Queries.Campaigns
{
    public class GetCampaignByNameQueryValidatorTests
    {
        private GetCampaignByNameQueryValidator _validator;

        public GetCampaignByNameQueryValidatorTests()
        {
            _validator = new GetCampaignByNameQueryValidator();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Name_HasDefaultValue_IsInvalid(string name)
        {
            // Arrange

            // Act
            var result = _validator.TestValidate(new GetCampaignByNameQuery(name));

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }
    }
}
