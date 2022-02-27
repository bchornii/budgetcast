using Azure.Messaging.ServiceBus;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.AzServiceBus.Extensions;
using Microsoft.Extensions.Logging;
using static BudgetCast.Common.Messaging.AzServiceBus.Events.EventBusConstants;

namespace BudgetCast.Common.Messaging.AzServiceBus.Events;

public class EventPublisher : IEventPublisher, IAsyncDisposable
{
    private readonly IMessagePreProcessor _messagePreProcessor;
    private readonly ILogger<EventPublisher> _logger;
    private readonly PluginSender _sender;

    public EventPublisher(
        EventBusClient eventBusClient, 
        IMessagePreProcessor messagePreProcessor,
        ILogger<EventPublisher> logger)
    {
        _messagePreProcessor = messagePreProcessor;
        _logger = logger;
        _sender = eventBusClient.Client.CreatePluginSender(TopicName);
    }
    
    public async Task Publish(IntegrationEvent @event, CancellationToken cancellationToken)
    {
        var eventName = GetEventName(@event.GetType());
        var json = _messagePreProcessor.PackAsJson(@event);

        var message = new ServiceBusMessage(body: json)
        {
            MessageId = @event.Id.ToString(),
            Subject = eventName,
        };
        await _sender.SendMessageAsync(message, cancellationToken);

        _logger.LogDebugIfEnabled(
            "Message with id = {Id} and payload = {Payload} has been published at {PublishedAt} UTC",
            message.MessageId, json, DateTime.UtcNow);

        if (!_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogInformationIfEnabled(
                "Message with id = {Id} has been published at {PublishedAt} UTC",
                message.MessageId, DateTime.UtcNow);
        }
    }
    
    public async ValueTask DisposeAsync() 
        => await _sender.DisposeAsync();

    private static string GetEventName<TEvent>()
        where TEvent : IntegrationEvent =>
        GetEventName(typeof(TEvent));

    private static string GetEventName(Type eventType)
        => eventType.Name;
}