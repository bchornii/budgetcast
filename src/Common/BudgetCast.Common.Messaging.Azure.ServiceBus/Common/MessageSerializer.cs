using System.Text.Json;
using BudgetCast.Common.Authentication;
using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Common;

/// <summary>
/// Custom implementation of integration message serializer which
/// supplements message metadata from <see cref="IIdentityContext"/>
/// </summary>
public class MessageSerializer : IMessageSerializer
{
    private readonly IIdentityContext _identityContext;
    private readonly ILogger<MessageSerializer> _logger;

    public MessageSerializer(IIdentityContext identityContext, ILogger<MessageSerializer> logger)
    {
        _identityContext = identityContext;
        _logger = logger;
    }
        
    public string PackAsJson(IntegrationMessage message)
    {
        var messageName = message.GetMessageName(); 
        if (_identityContext.HasAssociatedTenant && message.GetTenantId() is null)
        {
            message.SetCurrentTenant(_identityContext.TenantId!.Value);

            _logger.LogInformationIfEnabled("TenantId {TenantId} has been added to {MessageName} message with id {MessageId}", 
                _identityContext.TenantId, messageName, message.Id);
        }

        if (_identityContext.HasAssociatedUser && string.IsNullOrWhiteSpace(message.GetUserId()))
        {
            message.SetUserId(_identityContext.UserId);

            _logger.LogInformationIfEnabled("UserId {UserId} has been added to {MessageName} message with id {MessageId}", 
                _identityContext.UserId, messageName, message.Id);
        }

        return JsonSerializer.Serialize(message, message.GetType());
    }

    public object? UnpackFromJson(string message, Type messageType) 
        => JsonSerializer.Deserialize(message, messageType);
}