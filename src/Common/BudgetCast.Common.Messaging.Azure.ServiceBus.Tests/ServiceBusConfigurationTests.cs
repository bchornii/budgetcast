using AutoFixture;
using FluentAssertions;
using Xunit;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests;

public class ServiceBusConfigurationTests
{
    [Fact]
    public void Initialize_SubscriptionClientName_Should_Return_The_Same_Value()
    {
        // Arrange
        var options = new ServiceBusConfiguration();
        var fixture = new Fixture();

        var expectedSubscriptionClientName = fixture
            .Create<string>().ToLowerInvariant();
        
        // Act
        options.SubscriptionClientName = expectedSubscriptionClientName;

        // Assert
        options
            .SubscriptionClientName
            .Should()
            .Be(expectedSubscriptionClientName);
    }
    
    [Fact]
    public void Initialize_AzureServiceBusConnectionString_Should_Return_The_Same_Value()
    {
        // Arrange
        var options = new ServiceBusConfiguration();
        var fixture = new Fixture();

        var expectedAzureServiceBusConnectionString = fixture
            .Create<string>().ToLowerInvariant();
        
        // Act
        options.AzureServiceBusConnectionString = 
            expectedAzureServiceBusConnectionString;

        // Assert
        options
            .AzureServiceBusConnectionString
            .Should()
            .Be(expectedAzureServiceBusConnectionString);
    }
}