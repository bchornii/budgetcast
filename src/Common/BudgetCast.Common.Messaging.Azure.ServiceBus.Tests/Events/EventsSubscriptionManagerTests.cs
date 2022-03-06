using System;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events;

public class EventsSubscriptionManagerTests
{
    private readonly EventsSubscriptionManagerFixture _fixture;

    public EventsSubscriptionManagerTests()
    {
        _fixture = new EventsSubscriptionManagerFixture();
    }

    [Fact]
    public void No_Subscriptions_HasNoSubscriptions_Should_Return_True()
    {
        // Arrange
        _fixture
            .SubscriptionManager.RemoveAll();
        
        // Act
        var result = _fixture.SubscriptionManager.HasNoSubscriptions;

        // Assert
        result.Should().Be(true);
    }
    
    [Fact]
    public void Subscriptions_Added_HasNoSubscriptions_Should_Return_False()
    {
        // Arrange
        _fixture
            .SetupSubscriptions();
        
        // Act
        var result = _fixture.SubscriptionManager.HasNoSubscriptions;

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public void AddSubscription_Should_Add_Subscription()
    {
        // Arrange
        _fixture
            .SubscriptionManager
            .RemoveAll();

        // Act
        _fixture
            .SubscriptionManager
            .AddSubscription<FakeIntegrationEvent, FakeEventHandler1>();

        // Assert
        var result = _fixture.SubscriptionManager
            .HasSubscriptionsForEvent<FakeIntegrationEvent>();
        
        result.Should().Be(true);
    }
    
    [Fact]
    public void AddSubscription_To_The_Same_Handler_Should_Throw_Exception()
    {
        // Arrange
        var addEvent = _fixture
            .SubscriptionManager
            .AddSubscription<FakeIntegrationEvent, FakeEventHandler1>;
        
        _fixture
            .SubscriptionManager
            .RemoveAll();

        // Act
        addEvent();

        // Assert
        Assert.Throws<ArgumentException>(() =>
        {
            addEvent();
        });
    }
    
    [Fact]
    public void RemoveSubscription_Should_Remove_Subscription()
    {
        // Arrange
        _fixture
            .SubscriptionManager
            .AddSubscription<FakeIntegrationEvent, FakeEventHandler1>();
        
        // Act
        _fixture
            .SubscriptionManager
            .RemoveSubscription<FakeIntegrationEvent, FakeEventHandler1>();
        
        // Assert
        var result = _fixture.SubscriptionManager
            .HasSubscriptionsForEvent<FakeIntegrationEvent>();
        
        result.Should().Be(false);
    }
    
    [Fact]
    public void RemoveSubscription_Removes_One_Of_Two_Event_Subscription_Should_Not_Fire_OnEventRemoved_Event()
    {
        // Arrange
        var removedEventName = string.Empty;
        
        _fixture
            .SubscriptionManager
            .OnEventRemoved += (sender, eventName) => removedEventName = eventName;
        
        _fixture
            .SubscriptionManager
            .AddSubscription<FakeIntegrationEvent, FakeEventHandler1>();
        
        _fixture
            .SubscriptionManager
            .AddSubscription<FakeIntegrationEvent, FakeEventHandler2>();
        
        // Act
        _fixture
            .SubscriptionManager
            .RemoveSubscription<FakeIntegrationEvent, FakeEventHandler1>();
        
        // Assert
        removedEventName
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void RemoveSubscription_Non_Existing_Subscription_Should_Do_Nothing()
    {
        // Arrange
        
        // Act
        _fixture
            .SubscriptionManager
            .RemoveSubscription<FakeIntegrationEvent, FakeEventHandler1>();
        
        // Assert
    }
    
    [Fact]
    public void RemoveSubscription_Removes_Last_Event_Subscription_Should_Fire_OnEventRemoved_Event()
    {
        // Arrange
        var removedEventName = string.Empty;
        
        _fixture
            .SubscriptionManager
            .OnEventRemoved += (sender, eventName) => removedEventName = eventName;
        
        _fixture
            .SubscriptionManager
            .AddSubscription<FakeIntegrationEvent, FakeEventHandler1>();
        
        // Act
        _fixture
            .SubscriptionManager
            .RemoveSubscription<FakeIntegrationEvent, FakeEventHandler1>();
        
        // Assert
        removedEventName
            .Should()
            .Be(_fixture.SubscriptionManager.GetEventKey<FakeIntegrationEvent>());
    }

    [Fact]
    public void RemoveAll_Removes_All_Subscriptions()
    {
        // Arrange
        _fixture
            .SubscriptionManager
            .AddSubscription<FakeIntegrationEvent, FakeEventHandler1>();
        
        _fixture
            .SubscriptionManager
            .AddSubscription<FakeIntegrationEvent, FakeEventHandler2>();
        
        // Act
        _fixture.SubscriptionManager.RemoveAll();
        
        // Assert
        _fixture
            .SubscriptionManager
            .HasNoSubscriptions
            .Should()
            .Be(true);
    }

    [Fact]
    public void HasSubscriptionsForEvent_Generic_Should_Return_True_If_Event_Subscription_Exists()
    {
        // Arrange
        _fixture
            .SubscriptionManager
            .AddSubscription<FakeIntegrationEvent, FakeEventHandler1>();

        // Act
        var result = _fixture
            .SubscriptionManager
            .HasSubscriptionsForEvent<FakeIntegrationEvent>();

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public void HasSubscriptionsForEvent_Non_Generic_Should_Return_True_If_Event_Subscription_Exists()
    {
        // Arrange
        var eventName = _fixture
            .SubscriptionManager
            .GetEventKey<FakeIntegrationEvent>();
        
        _fixture
            .SubscriptionManager
            .AddSubscription<FakeIntegrationEvent, FakeEventHandler1>();

        // Act
        var result = _fixture
            .SubscriptionManager
            .HasSubscriptionsForEvent(eventName);

        // Assert
        result.Should().Be(true);
    }
    
    [Fact]
    public void GetEventKey_Should_Return_EventType_Name()
    {
        // Arrange
        
        // Act
        var result = _fixture
            .SubscriptionManager
            .GetEventKey<FakeIntegrationEvent>();

        // Assert
        result
            .Should()
            .Be(nameof(FakeIntegrationEvent));
    }

    [Fact]
    public void GetEventTypeByName_Should_Return_Correct_EventType_By_Name()
    {
        // Arrange
        var eventName = _fixture
            .SubscriptionManager
            .GetEventKey<FakeIntegrationEvent>();
        
        _fixture
            .SubscriptionManager
            .AddSubscription<FakeIntegrationEvent, FakeEventHandler1>();
        
        // Act
        var result = _fixture
            .SubscriptionManager
            .GetEventTypeByName(eventName);

        // Assert
        result
            .Should()
            .Be(typeof(FakeIntegrationEvent));
    }

    [Fact]
    public void GetHandlersForEvent_Generic_Should_Return_Correct_EventHandlers()
    {
        // Arrange
        _fixture
            .SubscriptionManager
            .AddSubscription<FakeIntegrationEvent, FakeEventHandler1>();
        
        _fixture
            .SubscriptionManager
            .AddSubscription<FakeIntegrationEvent, FakeEventHandler2>();
        
        // Act
        var result = _fixture
            .SubscriptionManager
            .GetHandlersForEvent<FakeIntegrationEvent>();

        // Assert
        result.Count.Should().Be(2);

        result
            .Should()
            .Contain(s => s.EventHandlerType == typeof(FakeEventHandler1));
        
        result
            .Should()
            .Contain(s => s.EventHandlerType == typeof(FakeEventHandler2));
    }
    
    [Fact]
    public void GetHandlersForEvent_Non_Generic_Should_Return_Correct_EventHandlers()
    {
        // Arrange
        var eventName = _fixture
            .SubscriptionManager
            .GetEventKey<FakeIntegrationEvent>();
        
        _fixture
            .SubscriptionManager
            .AddSubscription<FakeIntegrationEvent, FakeEventHandler1>();
        
        _fixture
            .SubscriptionManager
            .AddSubscription<FakeIntegrationEvent, FakeEventHandler2>();
        
        // Act
        var result = _fixture
            .SubscriptionManager
            .GetHandlersForEvent(eventName);

        // Assert
        result.Count.Should().Be(2);

        result
            .Should()
            .Contain(s => s.EventHandlerType == typeof(FakeEventHandler1));
        
        result
            .Should()
            .Contain(s => s.EventHandlerType == typeof(FakeEventHandler2));
    }
    
    private class EventsSubscriptionManagerFixture
    {
        private ILogger<EventsSubscriptionManager> Logger { get; }
        
        public EventsSubscriptionManager SubscriptionManager { get; }

        public EventsSubscriptionManagerFixture()
        {
            Logger = Mock.Of<ILogger<EventsSubscriptionManager>>();
            SubscriptionManager = new EventsSubscriptionManager(Logger);
        }

        public EventsSubscriptionManagerFixture SetupSubscriptions()
        {
            SubscriptionManager
                .AddSubscription<FakeIntegrationEvent, FakeEventHandler1>();
            
            SubscriptionManager
                .AddSubscription<FakeIntegrationEvent, FakeEventHandler2>();
            
            return this;
        }
    }
}