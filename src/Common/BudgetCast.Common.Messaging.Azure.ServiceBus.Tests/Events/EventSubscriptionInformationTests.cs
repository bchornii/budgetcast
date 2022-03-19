using System;
using AutoFixture;
using BudgetCast.Common.Messaging.Abstractions.Events;
using FluentAssertions;
using Xunit;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events;

public class EventSubscriptionInformationTests
{
    [Fact]
    public void Properties_Return_Values_Passed_In_Constructor_During_Initialization()
    {
        // Arrange
        var fixture = new Fixture();
        var eventType = fixture.Create<Type>();
        var eventHandlerType = fixture.Create<Type>();

        // Act
        var subscription = new EventSubscriptionInformation(eventHandlerType, eventType);

        // Assert
        subscription
            .EventType
            .Should()
            .Be(eventType);

        subscription
            .EventHandlerType
            .Should()
            .Be(eventHandlerType);
    }
    
    [Fact]
    public void ToString_Returns_Formatted_Subscription_Representation()
    {
        // Arrange
        var fixture = new Fixture();
        var eventType = fixture.Create<Type>();
        var eventHandlerType = fixture.Create<Type>();
        var subscription = new EventSubscriptionInformation(eventHandlerType, eventType);
        
        // Act
        var result = subscription.ToString();

        // Assert
        result
            .Should()
            .Be($"[{eventType.Name}-{eventHandlerType.Name}]");
    }

    [Fact]
    public void EventSubscriptionInformation_Null_Returns_Empty_Subscription()
    {
        // Arrange
        
        // Act
        var result = EventSubscriptionInformation.Null;

        // Assert
        result
            .EventType
            .Should()
            .BeNull();

        result
            .EventHandlerType
            .Should()
            .BeNull();
    }
}