using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using BudgetCast.Common.Messaging.Abstractions.Events;

namespace BudgetCast.Common.Application.Outbox;

public class IntegrationEventLogEntry
{
    private IntegrationEventLogEntry() {}

    public IntegrationEventLogEntry(IntegrationEvent @event, string scopeId) 
        : this(@event, transactionId: null, scopeId)
    {
    }
    
    public IntegrationEventLogEntry(IntegrationEvent @event, string? transactionId, string? scopeId)
    {
        EventId = @event.Id;
        CreationTime = @event.CreatedAt;
        EventTypeName = @event.GetType().FullName!;
        Content = JsonSerializer.Serialize(
            @event,
            @event.GetType(),
            new JsonSerializerOptions
            {
                WriteIndented = true
            });
        
        State = EventStateEnum.NotPublished;
        TimesSent = 0;
        TransactionId = transactionId;
        ScopeId = scopeId;
        IntegrationEvent = default!;
    }
    
    public string EventId { get; private set; }
    
    public string EventTypeName { get; private set; }
    
    [NotMapped]
    public string EventTypeShortName => EventTypeName.Split('.')?.Last()!;
    
    [NotMapped]
    public IntegrationEvent IntegrationEvent { get; private set; }
    
    public EventStateEnum State { get; set; }
    
    public int TimesSent { get; set; }
    
    public DateTime CreationTime { get; private set; }
    
    public string Content { get; private set; }
    
    public string? TransactionId { get; private set; }
    
    public string? ScopeId { get; private set; }

    public IntegrationEventLogEntry DeserializeJsonContent(Type type)
    {            
        IntegrationEvent = (IntegrationEvent)JsonSerializer.Deserialize(
            Content, type, new JsonSerializerOptions {PropertyNameCaseInsensitive = true})!;
        return this;
    }
}