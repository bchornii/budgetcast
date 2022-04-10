using Azure.Messaging.ServiceBus;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using Microsoft.Extensions.Logging;
using static BudgetCast.Common.Messaging.Azure.ServiceBus.Events.EventBusConstants;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Events;

/// <summary>
/// Event processing implementation based on <see cref="ServiceBusProcessor"/> API.
/// </summary>
public class EventsProcessor : IEventsProcessor, IAsyncDisposable
{
    public const int MaxDeliveryCount = 3;

    private readonly ServiceBusProcessor _processor;

    private readonly IEventsSubscriptionManager _subscriptionManager;
    private readonly ILogger<EventsProcessor> _logger;
    private readonly IMessageProcessingPipeline _processingPipeline;

    public EventsProcessor(
        IEventsSubscriptionManager subscriptionManager,
        ILogger<EventsProcessor> logger,
        string subscriptionClientName,
        IMessageProcessingPipeline processingPipeline,
        IEventBusClient eventBusClient,
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
            .CreateProcessor(topicName: TopicName, subscriptionName: subscriptionClientName, options);
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        SubscribeToMessageProcessing();
        SubscribeToErrorProcessing();

        _logger.LogInformationIfEnabled("Starting event processing at {StartingAt} UTC", DateTime.UtcNow);
        await _processor.StartProcessingAsync(cancellationToken);
        _logger.LogInformationIfEnabled("Event processing started at {StartedAt} UTC", DateTime.UtcNow);
    }

    public async Task Stop(CancellationToken cancellationToken)
    {
        _logger.LogInformationIfEnabled("Stopping event processing at {StartedAt} UTC", DateTime.UtcNow);
        await _processor.StopProcessingAsync(cancellationToken);
        _logger.LogInformationIfEnabled("Event processing stopped at {StartedAt} UTC", DateTime.UtcNow);
    }

    public Task SubscribeTo<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        _subscriptionManager.AddSubscription<TEvent, THandler>();
        return Task.CompletedTask;
    }

    public Task UnsubscribeFrom<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IEventHandler<TEvent>
    {
        _subscriptionManager.RemoveSubscription<TEvent, THandler>();
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
        => await _processor.DisposeAsync();

    private void SubscribeToMessageProcessing()
    {
        _processor.ProcessMessageAsync += async (args) =>
        {
            var eventName = args.Message.Subject;
            var eventId = args.Message.MessageId;
            var eventData = args.Message.Body.ToString();

            _logger.LogInformationIfEnabled("Received {EventName} event with id {EventId}", eventName, eventId);

            try
            {
                _logger.LogInformationIfEnabled("Starting execution of event processing pipeline for {EventName} event with id {EventId}", eventName, eventId);
                
                var isHandled = await _processingPipeline
                    .Handle(eventId, eventName, eventData, args.CancellationToken);

                if (isHandled)
                {
                    _logger.LogInformationIfEnabled(
                        "Finished execution of event processing pipeline for {EventName} event with id {EventId}", eventName, eventId);

                    _logger.LogInformationIfEnabled("Marking {EventName} event with id {EventId} as completed", eventName, eventId);
                    await args.CompleteMessageAsync(args.Message, args.CancellationToken);
                    _logger.LogInformationIfEnabled("Marked {EventName} event with id {EventId} as completed", eventName, eventId);
                }
                else
                {
                    _logger.LogError("Finished execution of event processing pipeline for {EventName} event with id {EventId} with error", eventName, eventId);

                    if (args.Message.DeliveryCount > MaxDeliveryCount)
                    {
                        await args.DeadLetterMessageAsync(
                            args.Message,
                            deadLetterReason: "MaxAppDeliveryCount",
                            deadLetterErrorDescription: "Application reached max preconfigured delivery count.",
                            cancellationToken: args.CancellationToken);

                        _logger.LogWarning("{EventName} event with id {EventId} dead lettered due to reaching max delivery count of {MaxDeliveryCount}", eventName, eventId, MaxDeliveryCount);
                    }
                }
            }
            catch (Exception ex)
            {
                await args.DeadLetterMessageAsync(
                    message: args.Message,
                    deadLetterReason: "EventProcessingException",
                    deadLetterErrorDescription: ex.ToString(),
                    cancellationToken: args.CancellationToken);

                _logger.LogWarning("{EventName} event with id {EventId} dead lettered due to exception in a event handling pipeline", eventName, eventId);
                
                throw;
            }
        };
    }

    private void SubscribeToErrorProcessing()
    {
        _processor.ProcessErrorAsync += (args) =>
        {
            var ex = args.Exception;
            var context = args.ErrorSource;

            _logger.LogError(ex, "ERROR handling event: {ExceptionMessage} - Context: {@ExceptionContext}", ex.Message, context);

            return Task.CompletedTask;
        };
    }
}