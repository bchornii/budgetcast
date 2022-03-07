using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events;

public class EventsProcessorTests
{
    private readonly EventsProcessorFixture _fixture;

    public EventsProcessorTests()
    {
        _fixture = new EventsProcessorFixture();
    }

    [Fact]
    public async Task Start_Processor_Should_Start_Processing()
    {
        // Arrange
        
        // Act
        await _fixture.Processor.Start(CancellationToken.None);

        // Assert
        _fixture
            .ServiceBusProcessor
            .WasProcessingStarted
            .Should()
            .Be(true);
    }
    
    [Fact]
    public async Task Stop_Processor_Should_Stop_Processing()
    {
        // Arrange
        
        // Act
        await _fixture.Processor.Stop(CancellationToken.None);

        // Assert
        _fixture
            .ServiceBusProcessor
            .WasProcessingStopped
            .Should()
            .Be(true);
    }

    [Fact]
    public async Task SubscribeTo_Should_Create_Subscription_In_SubscriptionManager()
    {
        // Arrange
        
        // Act
        await _fixture
            .Processor
            .SubscribeTo<FakeIntegrationEvent, FakeEventHandler1>();

        // Assert
        Mock.Get(_fixture.SubscriptionManager)
            .Verify(v => v
                .AddSubscription<FakeIntegrationEvent, FakeEventHandler1>(), Times.Once);
    }
    
    [Fact]
    public async Task UnsubscribeFrom_Should_Remove_Subscription_From_SubscriptionManager()
    {
        // Arrange
        
        // Act
        await _fixture
            .Processor
            .UnsubscribeFrom<FakeIntegrationEvent, FakeEventHandler1>();

        // Assert
        Mock.Get(_fixture.SubscriptionManager)
            .Verify(v => v
                .RemoveSubscription<FakeIntegrationEvent, FakeEventHandler1>(), Times.Once);
    }

    [Fact]
    public async Task OnProcessMessageAsync_Should_Call_ProcessingMessage_Callback()
    {
        // Arrange
        var eventName = nameof(FakeIntegrationEvent);
        var eventId = Guid.NewGuid();
        var eventIdAsString = eventId.ToString();
        var integrationEvent = new FakeIntegrationEvent(eventId);
        var payload = JsonSerializer.Serialize(integrationEvent);
        
        await _fixture.Processor.Start(CancellationToken.None);

        // Act
        await _fixture.ServiceBusProcessor.SimulateMessageReceivingOf(
            messageId: eventIdAsString,
            subject: eventName,
            payload);

        // Assert
        _fixture
            .ServiceBusProcessor
            .WasProcessingCallbackCalled
            .Should()
            .Be(true);
    }
    
    [Fact]
    public async Task OnProcessMessageAsync_Should_Trigger_Message_ProcessingPipeline()
    {
        // Arrange
        var eventName = nameof(FakeIntegrationEvent);
        var eventId = Guid.NewGuid();
        var eventIdAsString = eventId.ToString();
        var integrationEvent = new FakeIntegrationEvent(eventId);
        var payload = JsonSerializer.Serialize(integrationEvent);
        
        await _fixture.Processor.Start(CancellationToken.None);

        // Act
        await _fixture.ServiceBusProcessor.SimulateMessageReceivingOf(
            messageId: eventIdAsString,
            subject: eventName,
            payload);

        // Assert
        Mock.Get(_fixture.ProcessingPipeline)
            .Verify(v => v
                .Handle(eventIdAsString, eventName, payload, CancellationToken.None), Times.Once);
    }
    
    [Fact]
    public async Task OnProcessMessageAsync_ProcessingPipeline_Throws_Exception_Should_DeadLetter_Message()
    {
        // Arrange
        var eventName = nameof(FakeIntegrationEvent);
        var eventId = Guid.NewGuid();
        var eventIdAsString = eventId.ToString();
        var integrationEvent = new FakeIntegrationEvent(eventId);
        var payload = JsonSerializer.Serialize(integrationEvent);
        
        Mock.Get(_fixture.ProcessingPipeline)
            .Setup(s => s
                .Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
            .Throws(new Exception("Unit tests exception"));
        
        await _fixture.Processor.Start(CancellationToken.None);
        
        // Act
        var args = await _fixture.ServiceBusProcessor.SimulateMessageReceivingOf(
            messageId: eventIdAsString,
            subject: eventName,
            payload,
            rethrowExceptions: false);
        
        // Assert
        args
            .WasDeadLettered
            .Should()
            .Be(true);
    }
    
    [Fact]
    public async Task OnProcessMessageAsync_ProcessingPipeline_Throws_Exception_Should_Not_Swallow_It()
    {
        // Arrange
        var eventName = nameof(FakeIntegrationEvent);
        var eventId = Guid.NewGuid();
        var eventIdAsString = eventId.ToString();
        var integrationEvent = new FakeIntegrationEvent(eventId);
        var payload = JsonSerializer.Serialize(integrationEvent);
        
        Mock.Get(_fixture.ProcessingPipeline)
            .Setup(s => s
                .Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
            .Throws(new Exception("Unit tests exception"));
        
        await _fixture.Processor.Start(CancellationToken.None);
        
        // Act

        // Assert
        await Assert.ThrowsAsync<Exception>(async () =>
        {
            await _fixture.ServiceBusProcessor.SimulateMessageReceivingOf(
                messageId: eventIdAsString,
                subject: eventName,
                payload,
                rethrowExceptions: true);
        });
    }
    
    [Fact]
    public async Task OnProcessMessageAsync_MessageProcessing_Is_Successful_Should_Complete_Message()
    {
        // Arrange
        var eventName = nameof(FakeIntegrationEvent);
        var eventId = Guid.NewGuid();
        var eventIdAsString = eventId.ToString();
        var integrationEvent = new FakeIntegrationEvent(eventId);
        var payload = JsonSerializer.Serialize(integrationEvent);

        Mock.Get(_fixture.ProcessingPipeline)
            .Setup(s => s
                .Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(true);
        
        await _fixture.Processor.Start(CancellationToken.None);

        // Act
        var args = await _fixture.ServiceBusProcessor.SimulateMessageReceivingOf(
            messageId: eventIdAsString,
            subject: eventName,
            payload);

        // Assert
        args
            .WasCompleted
            .Should()
            .Be(true);
    }
    
    [Fact]
    public async Task OnProcessMessageAsync_MessageProcessing_Fails_And_MaxDeliveryCount_Not_Reached_Should_Skip_To_Next_Retry()
    {
        // Arrange
        var eventName = nameof(FakeIntegrationEvent);
        var eventId = Guid.NewGuid();
        var eventIdAsString = eventId.ToString();
        var integrationEvent = new FakeIntegrationEvent(eventId);
        var payload = JsonSerializer.Serialize(integrationEvent);

        Mock.Get(_fixture.ProcessingPipeline)
            .Setup(s => s
                .Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(false);
        
        await _fixture.Processor.Start(CancellationToken.None);
        
        // Act
        var args = await _fixture.ServiceBusProcessor.SimulateMessageReceivingOf(
            messageId: eventIdAsString,
            subject: eventName,
            payload);

        // Assert
        args
            .WasCompleted
            .Should()
            .Be(false);
        
        args
            .WasDeadLettered
            .Should()
            .Be(false);
    }
    
    [Fact]
    public async Task OnProcessMessageAsync_MessageProcessing_Fails_And_MaxDeliveryCount_Reached_Should_DeadLetter_Message()
    {
        // Arrange
        var eventName = nameof(FakeIntegrationEvent);
        var eventId = Guid.NewGuid();
        var eventIdAsString = eventId.ToString();
        var integrationEvent = new FakeIntegrationEvent(eventId);
        var payload = JsonSerializer.Serialize(integrationEvent);

        Mock.Get(_fixture.ProcessingPipeline)
            .Setup(s => s
                .Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(false);
        
        await _fixture.Processor.Start(CancellationToken.None);

        // Act
        var args = await _fixture.ServiceBusProcessor.SimulateMessageReceivingOf(
            messageId: eventIdAsString,
            subject: eventName,
            payload,
            attempt: EventsProcessor.MaxDeliveryCount + 1);

        // Assert
        args
            .WasDeadLettered
            .Should()
            .Be(true);
    }

    [Fact]
    public async Task OnProcessErrorAsync_Should_Call_Error_Processing_Callback()
    {
        // Arrange
        await _fixture.Processor.Start(CancellationToken.None);

        // Act
        await _fixture
            .ServiceBusProcessor
            .SimulateErrorReceiving();

        // Assert
        _fixture
            .ServiceBusProcessor
            .WasErrorCallbackCalled
            .Should()
            .Be(true);
    }
    
    private class EventsProcessorFixture
    {
        private IEventBusClient EventBusClient { get; }
        
        private FakeServiceBusClient ServiceBusClient { get; }
        
        public ILogger<EventsProcessor> Logger { get; }
        
        public IMessageProcessingPipeline ProcessingPipeline { get; }
        
        public IEventsSubscriptionManager SubscriptionManager { get; }
        
        public FakeServiceBusProcessor ServiceBusProcessor { get; }
        
        public EventsProcessor Processor { get; }

        public EventsProcessorFixture()
        {
            ServiceBusProcessor = new FakeServiceBusProcessor();
            ServiceBusClient = new FakeServiceBusClient(ServiceBusProcessor);

            SubscriptionManager = Mock.Of<IEventsSubscriptionManager>();
            Logger = Mock.Of<ILogger<EventsProcessor>>();
            ProcessingPipeline = Mock.Of<IMessageProcessingPipeline>();
            EventBusClient = Mock.Of<IEventBusClient>();
            Mock.Get(EventBusClient)
                .Setup(s => s.Client)
                .Returns(ServiceBusClient);

            Processor = new EventsProcessor(
                SubscriptionManager,
                Logger,
                nameof(EventsProcessor),
                ProcessingPipeline,
                EventBusClient);
        }
    }
}