using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Events;

// TODO: unit test
public class EventProcessingPipeline : IMessageProcessingPipeline
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventsSubscriptionManager _subscriptionManager;
    private readonly ILogger<EventProcessingPipeline> _logger;

    public EventProcessingPipeline(
        IServiceProvider serviceProvider, 
        IEventsSubscriptionManager subscriptionManager,
        ILogger<EventProcessingPipeline> logger)
    {
        _serviceProvider = serviceProvider;
        _subscriptionManager = subscriptionManager;
        _logger = logger;
    }
        
    public async Task<bool> Handle(
        string messageId, 
        string messageName, 
        string messageData,
        CancellationToken cancellationToken)
    {
        if (!_subscriptionManager.HasSubscriptionsForEvent(messageName))
        {
            _logger.LogWarning("Subscription for {EventName} event with id {EventId} has not been found", messageName, messageId);
            return false;
        }
            
        using var scope = _serviceProvider.CreateScope();
        var scopedServiceProvider = scope.ServiceProvider;
        var messageSerializer = scopedServiceProvider.GetRequiredService<IMessageSerializer>();

        _logger.LogInformationIfEnabled("Retrieving event subscriptions for {EventName} with {EventId}", messageName, messageId);
        var eventSubscriptions = _subscriptionManager.GetHandlersForEvent(messageName);
        _logger.LogInformationIfEnabled("Retrieved {TotalEventSubscriptions} event subscriptions for {EventName} with {EventId}", 
            eventSubscriptions.Count, messageName, messageId);
            
        foreach (var eventSubscription in eventSubscriptions)
        {
            var eventHandler = scopedServiceProvider
                .GetService(eventSubscription.EventHandlerType);
                
            if (eventHandler == null)
            {
                _logger.LogWarning("Handler for {Subscription} subscription can not be resolved from DI for event id {EventId}", eventSubscription, messageId);
                return false;
            }
                
            _logger.LogInformationIfEnabled("Handler for {Subscription} subscription has been resolved from DI for event id {EventId}", eventSubscription, messageId);

            var eventType = _subscriptionManager.GetEventTypeByName(messageName);
            var eventData = messageSerializer.UnpackFromJson(messageData, eventType);
                
            if (eventData is null)
            {
                _logger.LogError("{EventName} event with {EventId} can not be deserialized", messageName, messageId);
                return false;
            }

            var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            var eventHandlerTypeName = eventHandlerType.GetGenericTypeName();
            var eventHandlerMethod = eventHandlerType.GetMethod(nameof(IEventHandler<IntegrationEvent>.Handle));

            _logger.LogInformationIfEnabled("Started execution of {Subscription} subscription pre-processing steps for event with id {EventId}", eventSubscription, messageId);
            await ExecutePreProcessingSteps(scopedServiceProvider, eventData, cancellationToken);
            _logger.LogInformationIfEnabled("Finished execution of {Subscription} subscription pre-processing steps for event with id {EventId}", eventSubscription, messageId);

            _logger.LogInformationIfEnabled("Starting execution of {Subscription} subscription handler for event with id {EventId}", eventSubscription, messageId);
            await (Task)eventHandlerMethod!.Invoke(
                eventHandler,
                new[]
                {
                    eventData,
                    cancellationToken
                })!;
            _logger.LogInformationIfEnabled("Finished execution of {Subscription} subscription handler for event with {EventId}", eventHandlerTypeName, messageId);
                
            _logger.LogInformationIfEnabled("Started execution of {Subscription} subscription post-processing steps for event with id {EventId}", eventSubscription, messageId);
            await ExecutePostProcessingSteps(scopedServiceProvider, eventData, cancellationToken);
            _logger.LogInformationIfEnabled("Finished execution of {Subscription} subscription post-processing steps for event with id {EventId}", eventSubscription, messageId);
        }
            
        return true;
    }

    /// <summary>
    /// Resolves and execute any pre-handle steps registered in DI.
    /// </summary>
    /// <param name="serviceProvider">Scoped to message processing service provider.</param>
    /// <param name="integrationMessage">Received integration message - event or command.</param>
    /// <param name="cancellationToken">Cancellation token for cooperative cancellation.</param>
    /// <returns></returns>
    private async Task ExecutePreProcessingSteps(
        IServiceProvider serviceProvider,
        object? integrationMessage, 
        CancellationToken cancellationToken)
    {
        var preProcessingSteps = serviceProvider
            .GetServices<IMessagePreProcessingStep>()
            .ToArray();

        _logger.LogInformationIfEnabled(
            "Resolved {MessagePreProcessingStepsTotal} pre-processing steps",
            preProcessingSteps.Length);
            
        foreach (var step in preProcessingSteps.OrEmpty())
        {
            var message = integrationMessage as IntegrationMessage;
            var messageName = message.GetMessageName();
                
            _logger.LogInformationIfEnabled(
                "Started execution of {MessagePreProcessingStepName} pre-processing step for {EventName} event with id {EventId}",
                step.GetType().Name,
                messageName,
                message?.Id);

            await step.Execute(message!, cancellationToken);

            _logger.LogInformationIfEnabled(
                "Finished execution of {MessagePreProcessingStepName} pre-processing step for {EventName} event with id {EventId}",
                step.GetType().Name,
                messageName,
                message?.Id);
        }
    }

    /// <summary>
    /// Resolves & executes any post-handle steps registered in DI.
    /// </summary>
    /// <param name="serviceProvider">Scoped to message processing service provider.</param>
    /// <param name="integrationMessage">Received integration message - event or command.</param>
    /// <param name="cancellationToken">Cancellation token for cooperative cancellation.</param>
    /// <returns></returns>
    private async Task ExecutePostProcessingSteps(
        IServiceProvider serviceProvider, 
        object? integrationMessage, 
        CancellationToken cancellationToken)
    {
        var postProcessingSteps = serviceProvider
            .GetServices<IMessagePostProcessingStep>()
            .ToArray();

        _logger.LogInformationIfEnabled(
            "Resolved {PostProcessingStepsTotal} post-processing steps",
            postProcessingSteps.Length);
            
        foreach (var step in postProcessingSteps.OrEmpty())
        {
            var message = integrationMessage as IntegrationMessage;
            var messageName = message.GetMessageName();
                
            _logger.LogInformationIfEnabled(
                "Started execution of {MessagePreProcessingStepName} post-processing step for {EventName} event with id {EventId}",
                step.GetType().Name,
                messageName,
                message?.Id);

            await step.Execute(message!, cancellationToken);

            _logger.LogInformationIfEnabled(
                "Finished execution of {MessagePreProcessingStepName} post-processing step for {EventName} event with id {EventId}",
                step.GetType().Name,
                messageName,
                message?.Id);
        }
    }
}