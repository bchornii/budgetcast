using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events;

public class EventsPublisherTests
{
    private readonly EventsPublisherFixture _fixture;

    public EventsPublisherTests()
    {
        _fixture = new EventsPublisherFixture();
    }

    [Fact]
    public async Task Publish_Message_Cant_Be_Serialized_Should_Return_False()
    {
        // Arrange
        _fixture
            .SetupMessageSerializerToReturnNull();

        // Act
        var result = await _fixture.Publisher
            .Publish(new FakeIntegrationEvent(), CancellationToken.None);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public async Task Publish_Message_Serialized_Should_Be_Sent()
    {
        // Arrange
        _fixture
            .SetupMessageSerializerToReturnJson(out var json);
        
        var integrationEvent = new FakeIntegrationEvent();

        // Act
        var result = await _fixture.Publisher
            .Publish(integrationEvent, CancellationToken.None);
        
        // Assert
        result.Should().Be(true);

        _fixture.SentMessage
            .Subject
            .Should()
            .Be(nameof(FakeIntegrationEvent));

        _fixture.SentMessage
            .Body.ToString()
            .Should()
            .Be(json);

        _fixture.SentMessage
            .MessageId
            .Should()
            .Be(integrationEvent.Id.ToString());
    }
    
    private class EventsPublisherFixture
    {
        private IMessageSerializer MessageSerializer { get; }

        private ILogger<EventsPublisher> Logger { get; }

        private IEventBusClient EventBusClient { get; }

        private FakeServiceBusClient ServiceBusClient { get; }
        
        private FakeServiceBusSender ServiceBusSender { get; }
        
        public IReadOnlyCollection<IMessagePreSendingStep> PreSendingSteps { get; }

        public ServiceBusMessage SentMessage => ServiceBusSender.CachedMessage;
        
        public EventsPublisher Publisher { get; }

        public EventsPublisherFixture()
        {
            ServiceBusSender = new FakeServiceBusSender();
            ServiceBusClient = new FakeServiceBusClient(ServiceBusSender);

            PreSendingSteps = Array.Empty<IMessagePreSendingStep>();
            MessageSerializer = Mock.Of<IMessageSerializer>();
            Logger = Mock.Of<ILogger<EventsPublisher>>();
            EventBusClient = Mock.Of<IEventBusClient>();
            Mock.Get(EventBusClient)
                .Setup(s => s.Client)
                .Returns(ServiceBusClient);
            
            Publisher = new EventsPublisher(EventBusClient, MessageSerializer, Logger, PreSendingSteps);
        }

        public EventsPublisherFixture SetupMessageSerializerToReturnNull()
        {
            Mock.Get(MessageSerializer)
                .Setup(s => s
                    .PackAsJson(It.IsAny<IntegrationEvent>()))
                .Returns((string)null!);
            
            return this;
        }

        public EventsPublisherFixture SetupMessageSerializerToReturnJson(out string json)
        {
            json = GetIntegrationMessageJson();
            
            Mock.Get(MessageSerializer)
                .Setup(s => s
                    .PackAsJson(It.IsAny<IntegrationEvent>()))
                .Returns(json);
            
            return this;
        }
        
        private static string GetIntegrationMessageJson()
            => "{\"Id\":\"7e7bb340-326f-2345-1234-f6cb65133b54\",\"CreatedAt\":\"2022-03-04T00:00:00\",\"Metadata\":{\"UserId\":\"7e7bb340-326f-43aa-a65b-f6cb65133b54\", \"TenantId\": \"7643\"}}";
    }
}