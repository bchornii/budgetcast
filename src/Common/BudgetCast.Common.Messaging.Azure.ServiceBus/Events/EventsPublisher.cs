using Azure.Messaging.ServiceBus;
using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using Microsoft.Extensions.Logging;
using static BudgetCast.Common.Messaging.Azure.ServiceBus.Events.EventBusConstants;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Events;

/// <summary>
/// Event publisher implementation based on <see cref="ServiceBusSender"/> API.
/// </summary>
public class EventsPublisher : IEventsPublisher, IAsyncDisposable
{
    private readonly IMessageSerializer _messagePreProcessor;
    private readonly ILogger<EventsPublisher> _logger;
    private readonly ServiceBusSender _sender;

    public EventsPublisher(
        IEventBusClient eventBusClient, 
        IMessageSerializer messagePreProcessor,
        ILogger<EventsPublisher> logger)
    {
        _messagePreProcessor = messagePreProcessor;
        _logger = logger;
        _sender = eventBusClient.Client.CreateSender(TopicName);
    }
    
    public async Task<bool> Publish(IntegrationEvent @event, CancellationToken cancellationToken)
    {
        var eventName = @event.GetMessageName();
        var json = _messagePreProcessor.PackAsJson(@event);

        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }
        
        var message = new ServiceBusMessage(body: json)
        {
            MessageId = @event.Id.ToString(),
            Subject = eventName,
        };
        await _sender.SendMessageAsync(message, cancellationToken);

        _logger.LogDebugIfEnabled("Event {EventName} with id {EventId} and '{Payload}' payload has been published at {PublishedAt} UTC",
            message.Subject, message.MessageId, json, DateTime.UtcNow);

        if (!_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogInformationIfEnabled("Event {EventName} with id {EventId} has been published at {PublishedAt} UTC",
                message.Subject, message.MessageId, DateTime.UtcNow);
        }

        return true;
    }
    
    public async ValueTask DisposeAsync() 
        => await _sender.DisposeAsync();
}