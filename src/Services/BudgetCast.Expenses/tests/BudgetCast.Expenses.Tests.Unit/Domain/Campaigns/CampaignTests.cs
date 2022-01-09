using AutoFixture;
using BudgetCast.Expenses.Domain.Campaigns;
using FluentAssertions;
using System;
using Xunit;

namespace BudgetCast.Expenses.Tests.Unit.Domain.Campaigns
{
    public class CampaignTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Campaign_Creation_With_Empty_Name_Should_Throw_Error(string name)
        {
            // Arrange
            Action createCampaignWithEmptyName = () =>
            {
                _ = new Campaign(name);
            };

            // Act

            // Assert
            Assert.Throws<Exception>(createCampaignWithEmptyName);
        }

        [Fact]
        public void Campaign_Creation_With_CompletesAtDate_SmallerThan_StartsAtDate_Should_Throw_Error()
        {
            // Arrange
            Action createCampaignWithWrongDates = () =>
            {
                var completesAt = new DateTime(2000, 1, 1);
                var startsAt = new DateTime(2001, 1, 1);
                var name = new Fixture().Create<string>();
                _ = new Campaign(name, startsAt, completesAt);
            };

            // Act

            // Assert
            Assert.Throws<Exception>(createCampaignWithWrongDates);
        }

        [Fact]
        public void Campaign_Initialization_Should_Return_Correct_Data()
        {
            // Arrange
            var completesAt = new DateTime(2002, 1, 1);
            var startsAt = new DateTime(2001, 1, 1);
            var name = new Fixture().Create<string>();
            var campaign = new Campaign(name, startsAt, completesAt);

            // Act

            // Assert
            campaign.Name.Should().Be(name);
            campaign.StartsAt.Should().Be(startsAt);
            campaign.CompletesAt.Should().Be(completesAt);
        }
    }
}
