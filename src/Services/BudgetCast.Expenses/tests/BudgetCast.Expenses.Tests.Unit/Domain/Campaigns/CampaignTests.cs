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
        public void Campaign_Creation_With_Empty_Name_Should_Return_Error_Result(string name)
        {
            // Arrange

            // Act
            var result = Campaign.Create(name);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Campaign_Creation_With_CompletesAtDate_SmallerThan_StartsAtDate_Should_Return_Error_Result()
        {
            // Arrange
            var completesAt = new DateTime(2000, 1, 1);
            var startsAt = new DateTime(2001, 1, 1);
            var name = new Fixture().Create<string>();

            // Act
            var result = Campaign.Create(name, startsAt, completesAt);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Campaign_Initialization_Should_Return_Correct_Data()
        {
            // Arrange
            var completesAt = new DateTime(2002, 1, 1);
            var startsAt = new DateTime(2001, 1, 1);
            var name = new Fixture().Create<string>();

            // Act
            var campaign = Campaign.Create(name, startsAt, completesAt).Value;

            // Assert
            campaign.Name.Should().Be(name);
            campaign.StartsAt.Should().Be(startsAt);
            campaign.CompletesAt.Should().Be(completesAt);
        }
    }
}
