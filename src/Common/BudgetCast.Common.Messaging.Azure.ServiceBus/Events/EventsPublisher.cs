using Azure.Messaging.ServiceBus;
using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using Microsoft.Extensions.Logging;
using static BudgetCast.Common.Messaging.Azure.ServiceBus.Events.EventBusConstants;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Events;

public class EventsPublisher : IEventsPublisher, IAsyncDisposable
{
    private readonly IMessageSerializer _messagePreProcessor;
    private readonly ILogger<EventsPublisher> _logger;
    private readonly ServiceBusPluginSender _sender;

    public EventsPublisher(
        EventBusClient eventBusClient, 
        IMessageSerializer messagePreProcessor,
        ILogger<EventsPublisher> logger)
    {
        _messagePreProcessor = messagePreProcessor;
        _logger = logger;
        _sender = eventBusClient.Client.CreatePluginSender(TopicName);
    }
    
    public async Task Publish(IntegrationEvent @event, CancellationToken cancellationToken)
    {
        var eventName = @event.GetMessageName();
        var json = _messagePreProcessor.PackAsJson(@event);

        var message = new ServiceBusMessage(body: json)
        {
            MessageId = @event.Id.ToString(),
            Subject = eventName,
        };
        await _sender.SendMessageAsync(message, cancellationToken);

        _logger.LogDebugIfEnabled(
            "Event {EventName} with id {EventId} and '{Payload}' payload has been published at {PublishedAt} UTC",
            message.Subject,
            message.MessageId, 
            json, 
            DateTime.UtcNow);

        if (!_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogInformationIfEnabled(
                "Event {EventName} with id {EventId} has been published at {PublishedAt} UTC",
                message.Subject,
                message.MessageId, 
                DateTime.UtcNow);
        }
    }
    
    public async ValueTask DisposeAsync() 
        => await _sender.DisposeAsync();
}