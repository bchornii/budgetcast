using Azure.Messaging.ServiceBus;
using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using Microsoft.Extensions.Logging;
using static BudgetCast.Common.Messaging.Azure.ServiceBus.Events.EventBusConstants;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Events;

public class EventsProcessor : IEventsProcessor, IAsyncDisposable
{
    private const int MaxDeliveryCount = 3;

    private readonly ServiceBusPluginProcessor _processor;

    private readonly IEventsSubscriptionManager _subscriptionManager;
    private readonly ILogger<EventsProcessor> _logger;
    private readonly IMessageProcessingPipeline _processingPipeline;

    public EventsProcessor(
        IEventsSubscriptionManager subscriptionManager,
        ILogger<EventsProcessor> logger,
        string subscriptionClientName,
        IMessageProcessingPipeline processingPipeline,
        EventBusClient eventBusClient,
        ServiceBusProcessorOptions? serviceBusProcessorOptions = null)
    {
        _subscriptionManager = subscriptionManager;
        _logger = logger;
        _processingPipeline = processingPipeline;

        var options = serviceBusProcessorOptions ?? new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            ReceiveMode = ServiceBusReceiveMode.PeekLock,
            MaxConcurrentCalls = 1,
        };
        _processor = eventBusClient.Client
            .CreatePluginProcessor(TopicName, subscriptionClientName, options);
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        _processor.ProcessMessageAsync += async (args) =>
        {
            var eventName = args.Message.Subject;
            var messageData = args.Message.Body.ToString();

            _logger.LogInformationIfEnabled(
                "Received {EventName} event",
                eventName);

            try
            {
                _logger.LogInformationIfEnabled(
                    "Executing message {MessageId} handling pipeline", args.Message.MessageId);
                
                var isHandled = await _processingPipeline
                    .Handle(eventName, messageData, args.CancellationToken);

                if (isHandled)
                {
                    _logger.LogInformationIfEnabled(
                        "Message {MessageId} handling pipeline executed successfully",
                        args.Message.MessageId);
                    
                    await args.CompleteMessageAsync(args.Message, args.CancellationToken);
                    
                    _logger.LogInformationIfEnabled("Message {MessageId} marked as completed", args.Message.MessageId);
                }

                if (!isHandled && args.Message.DeliveryCount > MaxDeliveryCount)
                {
                    _logger.LogWarning(
                        "Message {MessageId} handling failed and delivery count max value reached", 
                        args.Message.MessageId);
                    
                    await args.DeadLetterMessageAsync(
                        args.Message,
                        deadLetterReason: "MaxAppDeliveryCount",
                        deadLetterErrorDescription: "Application reached max preconfigured delivery count.",
                        cancellationToken: args.CancellationToken);
                    
                    _logger.LogWarning(
                        "Message {MessageId} dead lettered", 
                        args.Message.MessageId);
                }
            }
            catch (Exception ex)
            {
                await args.DeadLetterMessageAsync(
                    message: args.Message,
                    deadLetterReason: "MessageProcessingException",
                    deadLetterErrorDescription: ex.ToString(),
                    cancellationToken: args.CancellationToken);

                _logger.LogWarning(
                    "Message {MessageId} dead lettered due to exception in a message handling pipeline",
                    args.Message.MessageId);
                
                throw;
            }
        };

        _processor.ProcessErrorAsync += (args) =>
        {
            var ex = args.Exception;
            var context = args.ErrorSource;

            _logger.LogError(ex, "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}",
                ex.Message, context);

            return Task.CompletedTask;
        };

        await _processor.StartProcessingAsync(cancellationToken);

        _logger.LogInformationIfEnabled(
            "Event processing started at {StartedAt} UTC", 
            DateTime.UtcNow);
    }

    public async Task Stop(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        
        _logger.LogInformationIfEnabled(
            "Event processing stopped at {StartedAt} UTC", 
            DateTime.UtcNow);
    }

    public Task SubscribeTo<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        var eventName = GetEventName<TEvent>();

        _subscriptionManager.AddSubscription<TEvent, THandler>();
        _logger.LogInformation("Subscribed {EventHandler} to {EventName}", typeof(THandler), eventName);

        return Task.CompletedTask;
    }

    public Task UnsubscribeFrom<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        var eventName = GetEventName<TEvent>();

        _subscriptionManager.RemoveSubscription<TEvent, THandler>();
        _logger.LogInformation("Unsubscribed {EventHandler} from event {EventName}", typeof(THandler), eventName);

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
        => await _processor.DisposeAsync();

    private static string GetEventName<TEvent>()
        where TEvent : IntegrationEvent =>
        GetEventName(typeof(TEvent));

    private static string GetEventName(Type eventType)
        => eventType.Name;
}