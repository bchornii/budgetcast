using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Common
{
    public class MessageProcessingPipeline : IMessageProcessingPipeline
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventsSubscriptionManager _subscriptionManager;
        private readonly ILogger<MessageProcessingPipeline> _logger;

        public MessageProcessingPipeline(
            IServiceProvider serviceProvider, 
            IEventsSubscriptionManager subscriptionManager,
            ILogger<MessageProcessingPipeline> logger)
        {
            _serviceProvider = serviceProvider;
            _subscriptionManager = subscriptionManager;
            _logger = logger;
        }

        public async Task<bool> Handle(string eventName, string messageData, CancellationToken cancellationToken)
        {
            if (!_subscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                _logger.LogWarning(
                    "Subscription managed has not found subscriptions for {EventName}", 
                    eventName);
                return false;
            }
            
            using var scope = _serviceProvider.CreateScope();
            var scopedServiceProvider = scope.ServiceProvider;

            var subscriptions = _subscriptionManager
                .GetHandlersForEvent(eventName);

            foreach (var subscription in subscriptions)
            {
                var handler = scopedServiceProvider
                    .GetService(subscription.HandlerType);
                
                if (handler == null)
                {
                    _logger.LogWarning(
                        "Handler of {HandlerType} type cannot be resolved from DI", 
                        subscription.HandlerType.Name);
                    return false;
                }

                var eventType = _subscriptionManager.GetEventTypeByName(eventName);
                var messagePreProcessor = scopedServiceProvider.GetRequiredService<IMessageSerializer>();
                var integrationEvent = messagePreProcessor.UnpackFromJson(messageData, eventType);

                var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
                var handleMethod = handlerType.GetMethod(nameof(IEventHandler<IntegrationEvent>.Handle));

                _logger.LogInformationIfEnabled(
                    "Started execution of pre handling steps for {EventName}",
                    eventName);
                await ExecutePreHandlingSteps(
                    scopedServiceProvider,
                    integrationEvent, 
                    cancellationToken);
                _logger.LogInformationIfEnabled(
                    "Finished execution of pre handling steps for {EventName}",
                    eventName);

                _logger.LogInformationIfEnabled(
                    "Starting execution of handler {HandlerName} for {EventName}",
                    handlerType.GetGenericTypeName(),
                    eventName);
                await (Task)handleMethod!.Invoke(
                    handler,
                    new[]
                    {
                        integrationEvent,
                        cancellationToken
                    })!;
                _logger.LogInformationIfEnabled(
                    "Finished execution of handler {HandlerName} for {EventName}",
                    handlerType.GetGenericTypeName(),
                    eventName);
                
                _logger.LogInformationIfEnabled(
                    "Started execution of post handling steps for {EventName}",
                    eventName);
                await ExecutePostHandlingSteps(
                    scopedServiceProvider, 
                    integrationEvent, 
                    cancellationToken);
                _logger.LogInformationIfEnabled(
                    "Finished execution of post handling steps for {EventName}",
                    eventName);
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
        private async Task ExecutePreHandlingSteps(
            IServiceProvider serviceProvider,
            object? integrationMessage, 
            CancellationToken cancellationToken)
        {
            var preHandlingSteps = serviceProvider
                .GetServices<IMessagePreProcessingStep>()
                .ToArray();

            _logger.LogInformationIfEnabled(
                "Resolved {PreHandlingStepsTotal} pre handling steps",
                preHandlingSteps.Length);
            
            foreach (var step in preHandlingSteps.OrEmpty())
            {
                var message = integrationMessage as IntegrationMessage;
                await step.Execute(message!, cancellationToken);
            }
        }

        /// <summary>
        /// Resolves & executes any post-handle steps registered in DI.
        /// </summary>
        /// <param name="serviceProvider">Scoped to message processing service provider.</param>
        /// <param name="integrationMessage">Received integration message - event or command.</param>
        /// <param name="cancellationToken">Cancellation token for cooperative cancellation.</param>
        /// <returns></returns>
        private async Task ExecutePostHandlingSteps(
            IServiceProvider serviceProvider, 
            object? integrationMessage, 
            CancellationToken cancellationToken)
        {
            var postHandlingSteps = serviceProvider
                .GetServices<IMessagePostProcessingStep>()
                .ToArray();

            _logger.LogInformationIfEnabled(
                "Resolved {PostHandlingStepsTotal} post handling steps",
                postHandlingSteps.Length);
            
            foreach (var step in postHandlingSteps.OrEmpty())
            {
                var message = integrationMessage as IntegrationMessage;
                await step.Execute(message!, cancellationToken);
            }
        }
    }
}
