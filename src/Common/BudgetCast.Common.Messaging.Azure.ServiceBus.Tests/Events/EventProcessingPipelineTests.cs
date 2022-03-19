using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events.Fakes;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events;

public class EventProcessingPipelineTests
{
    private readonly EventProcessingPipelineFixture _fixture;

    public EventProcessingPipelineTests()
    {
        _fixture = new EventProcessingPipelineFixture()
            .InitDefaultStubs();
    }
    
    [Fact]
    public async Task Handle_No_Subscriptions_Found_Should_Return_False()
    {
        // Arrange
        _fixture
            .RemoveAllEventSubscriptions();
        
        // Act
        var result = await _fixture.Pipeline.Handle(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(),
            CancellationToken.None);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public async Task Handle_Has_Subscriptions_But_Handler_Cant_Be_Resolved_Should_Return_False()
    {
        // Arrange
        _fixture
            .SetupEventSubscriptions().But()
            .RemoveHandlersForPreConfiguredSubscriptions();

        // Act
        var result = await _fixture.Pipeline.Handle(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(),
            CancellationToken.None);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public async Task Handle_Handler_Resolved_But_EventData_Cant_Be_Deserialized_Should_Return_False()
    {
        // Arrange
        _fixture
            .SetupEventSubscriptions().And()
            .SetupHandlersForPreConfiguredSubscriptions().But()
            .SetupMessageSerializerToReturnNulls();
        
        // Act
        var result = await _fixture.Pipeline.Handle(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(),
            CancellationToken.None);
        
        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public async Task Handle_Has_PreProcessing_Steps_Should_Execute_Them()
    {
        // Arrange
        _fixture
            .SetupEventSubscriptions().And()
            .SetupHandlersForPreConfiguredSubscriptions().And()
            .SetupMessageSerializerToReturnMessages().And()
            .SetupMessagePreProcessingSteps().And()
            .SetupMessagePostProcessingSteps();

        // Act
        await _fixture.Pipeline.Handle(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(),
            CancellationToken.None);
        
        // Assert
        var numOfTimes = Times.Exactly(
            _fixture.Memento.Subscriptions.Count *
            _fixture.Memento.PreProcessingSteps.Count);
        
        foreach (var step in _fixture.Memento.PreProcessingSteps)
        {
            Mock.Get(step)
                .Verify(
                    s => s.Execute(
                        It.IsAny<IntegrationMessage>(),
                        CancellationToken.None), 
                    numOfTimes);
        }
        
        _fixture
            .Memento
            .ProcessedMessages
            .Select(m => m.State.IsPreProcessed)
            .Should()
            .AllBeEquivalentTo(true);
    }
    
    [Fact]
    public async Task Handle_Should_Execute_Resolved_Handler_To_Process_Message()
    {
        // Arrange
        _fixture
            .SetupEventSubscriptions().And()
            .SetupHandlersForPreConfiguredSubscriptions().And()
            .SetupMessageSerializerToReturnMessages().And()
            .SetupMessagePreProcessingSteps().And()
            .SetupMessagePostProcessingSteps();
        
        // Act
        await _fixture.Pipeline.Handle(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(),
            CancellationToken.None);
        
        // Assert
        _fixture
            .Memento
            .ProcessedMessages
            .Select(m => m.State.IsProcessed)
            .Should()
            .AllBeEquivalentTo(true);
    }
    
    [Fact]
    public async Task Handle_PostProcessing_Steps_Found_Should_Execute_All_Of_Them()
    {
        // Arrange
        _fixture
            .SetupEventSubscriptions().And()
            .SetupHandlersForPreConfiguredSubscriptions().And()
            .SetupMessageSerializerToReturnMessages().And()
            .SetupMessagePreProcessingSteps().And()
            .SetupMessagePostProcessingSteps();

        // Act
        await _fixture.Pipeline.Handle(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(),
            CancellationToken.None);
        
        // Assert
        var numOfTimes = Times.Exactly(
            _fixture.Memento.Subscriptions.Count *
            _fixture.Memento.PostProcessingSteps.Count);
        
        foreach (var step in _fixture.Memento.PostProcessingSteps)
        {
            Mock.Get(step)
                .Verify(
                    s => s.Execute(
                        It.IsAny<IntegrationMessage>(),
                        CancellationToken.None),
                    numOfTimes);
        }
        
        _fixture
            .Memento
            .ProcessedMessages
            .Select(m => m.State.IsPostProcessed)
            .Should()
            .AllBeEquivalentTo(true);
    }
    
    private class EventProcessingPipelineFixture
    {
        private IServiceScope Scope { get; }

        private ILogger<EventProcessingPipeline> Logger { get; }
        
        private IServiceProvider RootServiceProvider { get; }
        
        private IServiceProvider ScopedServiceProvider { get; }

        private IEventsSubscriptionManager SubscriptionManager { get; }

        private IMessageSerializer MessageSerializer { get; }
        
        public EventProcessingPipelineMemento Memento { get; }

        public EventProcessingPipeline Pipeline { get; }

        public EventProcessingPipelineFixture()
        {
            Memento = new EventProcessingPipelineMemento();
            RootServiceProvider = Mock.Of<IServiceProvider>();
            ScopedServiceProvider = Mock.Of<IServiceProvider>();
            Scope = Mock.Of<IServiceScope>();
            SubscriptionManager = Mock.Of<IEventsSubscriptionManager>();
            Logger = Mock.Of<ILogger<EventProcessingPipeline>>();
            MessageSerializer = Mock.Of<IMessageSerializer>();
            Pipeline = new EventProcessingPipeline(RootServiceProvider, SubscriptionManager, Logger);
        }

        public EventProcessingPipelineFixture InitDefaultStubs()
        {
            Mock.Get(Scope)
                .Setup(s => s.ServiceProvider)
                .Returns(ScopedServiceProvider);

            Mock.Get(ScopedServiceProvider)
                .Setup(s => s.GetService(typeof(IMessageSerializer)))
                .Returns(MessageSerializer);

            var serviceScopeFactory = Mock.Of<IServiceScopeFactory>();
            Mock.Get(serviceScopeFactory)
                .Setup(s => s.CreateScope())
                .Returns(Scope);

            Mock.Get(RootServiceProvider)
                .Setup(s => s.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory);
            
            return this;
        }

        /// <summary>
        /// Removes all event subscriptions.
        /// </summary>
        /// <returns></returns>
        public EventProcessingPipelineFixture RemoveAllEventSubscriptions()
        {
            Mock.Get(SubscriptionManager)
                .Setup(s => s
                    .HasSubscriptionsForEvent(It.IsAny<string>()))
                .Returns(false);
            
            return this;
        }
        
        /// <summary>
        /// Setup subscriptions for integration events and handlers under test and saves
        /// them in memento state, so they can be accessed later on.
        /// </summary>
        /// <returns></returns>
        public EventProcessingPipelineFixture SetupEventSubscriptions()
        {
            var subscription1 = new EventSubscriptionInformation(
                eventHandlerType: typeof(FakeEventHandler1),
                eventType: typeof(FakeIntegrationEvent));
            
            var subscription2 = new EventSubscriptionInformation(
                eventHandlerType: typeof(FakeEventHandler2),
                eventType: typeof(FakeIntegrationEvent));

            var eventSubscriptions = new[]
            {
                subscription1, 
                subscription2,
            };
            
            Mock.Get(SubscriptionManager)
                .Setup(s => s
                    .HasSubscriptionsForEvent(It.IsAny<string>()))
                .Returns(true);
            
            Mock.Get(SubscriptionManager)
                .Setup(s => s
                    .GetEventTypeByName(It.IsAny<string>()))
                .Returns(typeof(FakeIntegrationEvent));

            Mock.Get(SubscriptionManager)
                .Setup(s => s
                    .GetHandlersForEvent(It.IsAny<string>()))
                .Returns(eventSubscriptions);

            Memento
                .Save(eventSubscriptions);
            
            return this;
        }

        /// <summary>
        /// Setup handlers for pre-configured subscriptions, so correct handler instance is resolved
        /// during message processing.
        /// </summary>
        /// <returns></returns>
        public EventProcessingPipelineFixture SetupHandlersForPreConfiguredSubscriptions()
        {
            foreach (var subscription in Memento.Subscriptions)
            {
                var handler = Activator
                    .CreateInstance(subscription.EventHandlerType);

                Mock.Get(ScopedServiceProvider)
                    .Setup(s => s
                        .GetService(subscription.EventHandlerType))
                    .Returns(handler);
            }

            return this;
        }

        /// <summary>
        /// Removes handlers for pre-configured subscriptions.
        /// </summary>
        /// <returns></returns>
        public EventProcessingPipelineFixture RemoveHandlersForPreConfiguredSubscriptions()
        {
            foreach (var subscription in Memento.Subscriptions)
            {
                Mock.Get(ScopedServiceProvider)
                    .Setup(s => s
                        .GetService(subscription.EventHandlerType))
                    .Returns(null);
            }

            return this;
        }
        
        /// <summary>
        /// Setup message serializer to return instance of <see cref="FakeIntegrationEvent"/>
        /// and save in memento state, so it can be accessed later on.
        /// </summary>
        /// <returns></returns>
        public EventProcessingPipelineFixture SetupMessageSerializerToReturnMessages()
        {
            var message = new FakeIntegrationEvent();
            
            Mock.Get(MessageSerializer)
                .Setup(s => s
                    .UnpackFromJson(It.IsAny<string>(), It.IsAny<Type>()))
                .Returns(message);
            
            Memento
                .Save(message);

            return this;
        }

        /// <summary>
        /// Setup message serializer to return nulls for any JSON payload.
        /// </summary>
        /// <returns></returns>
        public EventProcessingPipelineFixture SetupMessageSerializerToReturnNulls()
        {
            Mock.Get(MessageSerializer)
                .Setup(s => s
                    .UnpackFromJson(It.IsAny<string>(), It.IsAny<Type>()))
                .Returns(null);
            
            return this;
        }

        /// <summary>
        /// Setup message pre-processing step mocks and saves them in the memento state,
        /// so they can be accessed later on.
        /// </summary>
        /// <returns></returns>
        public EventProcessingPipelineFixture SetupMessagePreProcessingSteps()
        {
            var messagePreProcessingStep = Mock.Of<IMessagePreProcessingStep>();

            Mock.Get(messagePreProcessingStep)
                .Setup(s => s
                    .Execute(It.IsAny<IntegrationMessage>(), CancellationToken.None))
                .Callback<IntegrationMessage, CancellationToken>(
                    (message, _) =>
                    {
                        var fakeMsg = (FakeIntegrationEvent)message;
                        fakeMsg.State.IsPreProcessed = true;
                    });
            
            var messagePreProcessingSteps = Enumerable
                .Repeat(messagePreProcessingStep, 2);

            Mock.Get(ScopedServiceProvider)
                .Setup(s => s
                    .GetService(typeof(IEnumerable<IMessagePreProcessingStep>)))
                .Returns(messagePreProcessingSteps);

            Memento
                .Save(messagePreProcessingSteps.ToArray());

            return this;
        }
        
        /// <summary>
        /// Setup message post-processing step mocks and saves them in the memento state,
        /// so they can be accessed later on.
        /// </summary>
        /// <returns></returns>
        public EventProcessingPipelineFixture SetupMessagePostProcessingSteps()
        {
            var messagePostProcessingStep = Mock.Of<IMessagePostProcessingStep>();
            
            Mock.Get(messagePostProcessingStep)
                .Setup(s => s
                    .Execute(It.IsAny<IntegrationMessage>(), CancellationToken.None))
                .Callback<IntegrationMessage, CancellationToken>(
                    (message, _) =>
                    {
                        var fakeMsg = (FakeIntegrationEvent)message;
                        fakeMsg.State.IsPostProcessed = true;
                    });
            
            var messagePostProcessingSteps = Enumerable
                .Repeat(messagePostProcessingStep, 2);

            Mock.Get(ScopedServiceProvider)
                .Setup(s => s
                    .GetService(typeof(IEnumerable<IMessagePostProcessingStep>)))
                .Returns(messagePostProcessingSteps);

            Memento
                .Save(messagePostProcessingSteps.ToArray());

            return this;
        }

        public EventProcessingPipelineFixture And()
            => this;

        public EventProcessingPipelineFixture But()
            => this;
        
        /// <summary>
        /// Memento associated with <see cref="EventProcessingPipelineFixture"/> processing
        /// state. Used to save intermediate results and setups which were made on the fixture itself.
        /// </summary>
        internal class EventProcessingPipelineMemento
        {
            private readonly List<FakeIntegrationEvent> _messages;

            public EventProcessingPipelineMemento()
            {
                _messages = new List<FakeIntegrationEvent>();
            }

            /// <summary>
            /// Returns collection of message <see cref="IMessagePostProcessingStep"/> steps
            /// configured by fixture
            /// for tests.
            /// </summary>
            public IReadOnlyList<IMessagePostProcessingStep> PostProcessingSteps { get; private set; } = default!;

            /// <summary>
            /// Returns collection of message <see cref="IMessagePreProcessingStep"/> steps
            /// configured by fixture
            /// for tests.
            /// </summary>
            public IReadOnlyList<IMessagePreProcessingStep> PreProcessingSteps { get; private set; } = default!;

            /// <summary>
            /// Returns collection of <see cref="EventSubscriptionInformation"/> subscriptions
            /// configured by fixture for tests.
            /// </summary>
            public IReadOnlyList<EventSubscriptionInformation> Subscriptions { get; private set; } = default!;

            /// <summary>
            /// Returns collection of processed during the tests <see cref="FakeIntegrationEvent"/>
            /// messages.
            /// </summary>
            public IReadOnlyList<FakeIntegrationEvent> ProcessedMessages => _messages;

            /// <summary>
            /// Saves collection of <see cref="IMessagePostProcessingStep"/> steps in memento
            /// state.
            /// </summary>
            /// <param name="postProcessingSteps"></param>
            public void Save(IReadOnlyList<IMessagePostProcessingStep> postProcessingSteps)
                => PostProcessingSteps = postProcessingSteps;

            /// <summary>
            /// Saves collection of <see cref="IMessagePreProcessingStep"/> steps in memento
            /// state.
            /// </summary>
            /// <param name="preProcessingSteps"></param>
            public void Save(IReadOnlyList<IMessagePreProcessingStep> preProcessingSteps)
                => PreProcessingSteps = preProcessingSteps;

            /// <summary>
            /// Saves collection of <see cref="EventSubscriptionInformation"/> subscriptions
            /// in memento state.
            /// </summary>
            /// <param name="subscriptions"></param>
            public void Save(IReadOnlyList<EventSubscriptionInformation> subscriptions)
                => Subscriptions = subscriptions;

            /// <summary>
            /// Saves <see cref="FakeIntegrationEvent"/> message in collection of
            /// processed messages in memento state.
            /// </summary>
            /// <param name="message"></param>
            public void Save(FakeIntegrationEvent message)
                => _messages.Add(message);
        }
    }
}