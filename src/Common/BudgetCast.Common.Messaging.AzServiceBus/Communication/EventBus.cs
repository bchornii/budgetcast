using Azure.Messaging.ServiceBus;
using BudgetCast.Common.Messaging.Abstractions.Communication;
using BudgetCast.Common.Messaging.Abstractions.Handlers;
using BudgetCast.Common.Messaging.Abstractions.Messages;
using BudgetCast.Common.Messaging.Abstractions.Messages.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BudgetCast.Common.Messaging.AzServiceBus.Communication
{
    public class EventBus : IEventBus, IAsyncDisposable
    {
        private const string IntegrationEventSuffix = "IntegrationEvent";
        private readonly string _topicName = "budgetcast_events_topic";

        private PluginSender _sender;
        private PluginProcessor _processor;

        private readonly IEventSubscriptionManager _subscriptionManager;
        private readonly ILogger<EventBus> _logger;
        private readonly IIntegrationMessageProcessingPipeline _processingPipeline;

        public EventBus(
            IEventSubscriptionManager subscriptionManager,
            ILogger<EventBus> logger,
            string subscriptionClientName,
            ServiceBusProcessorOptions serviceBusProcessorOptions, 
            IIntegrationMessageProcessingPipeline processingPipeline)
        {
            _subscriptionManager = subscriptionManager;
            _logger = logger;
            _processingPipeline = processingPipeline;

            // TODO: rework
            var client = new ServiceBusClient("sdgfdfg");

            var options = serviceBusProcessorOptions ?? new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                MaxConcurrentCalls = 1,
            };
            _processor = client
                .CreatePluginProcessor(_topicName, subscriptionClientName, options);

            _sender = client
                .CreatePluginSender(_topicName);
        }

        public async Task Publish(IntegrationEvent @event, CancellationToken cancellationToken)
        {
            var eventName = GetEventName(@event.GetType());
            var jsonMessage = JsonSerializer.Serialize(@event, @event.GetType());

            var message = new ServiceBusMessage(body: jsonMessage)
            {
                MessageId = @event.Id.ToString(),
                Subject = eventName,
            };
            await _sender.SendMessageAsync(message, cancellationToken);
        }

        public Task Subscribe<TEvent, THandler>(CancellationToken cancellationToken)
            where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = GetEventName<TEvent>();

            _subscriptionManager.AddSubscription<TEvent, THandler>();
            _logger.LogInformation("Subscribed {EventHandler} to {EventName}", typeof(THandler), eventName);

            return Task.CompletedTask;
        }

        public Task Unsubscribe<TEvent, THandler>(CancellationToken cancellationToken)
            where TEvent : IntegrationEvent
            where THandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = GetEventName<TEvent>();

            _subscriptionManager.RemoveSubscription<TEvent, THandler>();
            _logger.LogInformation("Unsubscribed {EventHandler} from event {EventName}", typeof(THandler), eventName);

            return Task.CompletedTask;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            _processor.ProcessMessageAsync += async (args) =>
            {
                var eventName = $"{args.Message.Subject}{IntegrationEventSuffix}";
                var messageData = args.Message.Body.ToString();

                var success = await _processingPipeline
                    .Process(eventName, messageData, args.CancellationToken);

                if (success)
                {
                    await args.CompleteMessageAsync(args.Message, args.CancellationToken);
                }
            };

            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync();
        }

        public async Task Stop(CancellationToken cancellationToken) 
            => await _processor.StopProcessingAsync(cancellationToken);

        public async ValueTask DisposeAsync()
        {
            await _sender.DisposeAsync();
            await _processor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            var ex = args.Exception;
            var context = args.ErrorSource;

            _logger.LogError(ex, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", ex.Message, context);

            return Task.CompletedTask;
        }

        private static string GetEventName<TEvent>()
            where TEvent : IntegrationEvent =>
            GetEventName(typeof(TEvent));

        private static string GetEventName(Type eventType)
            => eventType.Name.Replace(IntegrationEventSuffix, string.Empty);
    }

    public class IntegrationMessageHandlingPipeline : IIntegrationMessageProcessingPipeline
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventSubscriptionManager _subscriptionManager;

        public IntegrationMessageHandlingPipeline(
            IServiceProvider serviceProvider, 
            IEventSubscriptionManager subscriptionManager)
        {
            _serviceProvider = serviceProvider;
            _subscriptionManager = subscriptionManager;
        }

        public async Task<bool> Process(string eventName, string messageData, CancellationToken cancellationToken)
        {
            if (_subscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var subscriptions = _subscriptionManager
                        .GetHandlersForEvent(eventName);

                    foreach (var subscription in subscriptions)
                    {
                        var handler = scope.ServiceProvider
                            .GetService(subscription.HandlerType);
                        if (handler == null)
                        {
                            return false;
                        }

                        var eventType = _subscriptionManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonSerializer.Deserialize(messageData, eventType);

                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        var handleMethod = concreteType.GetMethod(
                            nameof(IIntegrationEventHandler<IntegrationEvent>.Handle));

                        await RunPreHandlerSteps(integrationEvent, cancellationToken);

                        await (Task)handleMethod!.Invoke(
                            handler,
                            new[]
                            {
                                integrationEvent,
                                cancellationToken
                            })!;

                        await RunPostHandlerSteps(integrationEvent, cancellationToken);
                    }
                }

                return true;
            }

            return false;
        }

        private async Task RunPreHandlerSteps(object? integrationMessage, CancellationToken cancellationToken)
        {
            var preHandlerSteps = _serviceProvider
                .GetServices<IIntegrationMessagePreHandlerStep>();
            foreach (var preHandlerStep in preHandlerSteps.OrEmpty())
            {
                var message = integrationMessage as IntegrationMessage;
                await preHandlerStep.Execute(message!, cancellationToken);
            }
        }

        private async Task RunPostHandlerSteps(object? integrationMessage, CancellationToken cancellationToken)
        {
            var postHandlerSteps = _serviceProvider
                .GetServices<IIntegrationMessagePreHandlerStep>();

            foreach (var postHandlerStep in postHandlerSteps.OrEmpty())
            {
                var message = integrationMessage as IntegrationMessage;
                await postHandlerStep.Execute(message!, cancellationToken);
            }
        }
    }

    public class EventBusStatup : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public EventBusStatup(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var eventBus = _serviceProvider.GetRequiredService<IEventBus>();
            await eventBus.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var eventBus = _serviceProvider.GetRequiredService<IEventBus>();
            await eventBus.Stop(cancellationToken);
        }
    }
}
