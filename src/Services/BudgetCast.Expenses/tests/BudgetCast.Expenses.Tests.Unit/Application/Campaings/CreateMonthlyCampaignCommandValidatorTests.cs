using BudgetCast.Expenses.Commands.Campaigns;
using BudgetCast.Expenses.Domain.Campaigns;
using FluentValidation.TestHelper;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Application.Campaings
{
    public class CreateMonthlyCampaignCommandValidatorTests
    {
        private CreateMonthlyCampaignCommandValidatorFixture _fixture;

        public CreateMonthlyCampaignCommandValidatorTests()
        {
            _fixture = new CreateMonthlyCampaignCommandValidatorFixture();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Name_Is_Empty_Should_Be_Invalid(string name)
        {
            // Arrange

            // Act
            var result = _fixture.Validator.TestValidate(
                new CreateMonthlyCampaignCommand
                {
                    Name = name
                });

            // Assert            
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public async Task Name_Exists_Should_Be_Invalid()
        {
            // Arrange
            Mock.Get(_fixture.CampaignRepository)
                .Setup(s => s.ExistsAsync(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(true);

            // Act
            var result = await _fixture.Validator.TestValidateAsync(
                new CreateMonthlyCampaignCommand
                {
                    Name = "some name"
                });

            // Assert            
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public async Task Name_Not_Exist_Should_Be_Invalid()
        {
            // Arrange
            Mock.Get(_fixture.CampaignRepository)
                .Setup(s => s.ExistsAsync(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync(false);

            // Act
            var result = await _fixture.Validator.TestValidateAsync(
                new CreateMonthlyCampaignCommand
                {
                    Name = "some name"
                });

            // Assert            
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        private class CreateMonthlyCampaignCommandValidatorFixture
        {
            public ICampaignRepository CampaignRepository { get; }

            public CreateMonthlyCampaignCommandValidator Validator { get; }

            public CreateMonthlyCampaignCommandValidatorFixture()
            {
                CampaignRepository = Mock.Of<ICampaignRepository>();
                Validator = new CreateMonthlyCampaignCommandValidator(CampaignRepository);
            }
        }
    }
}
