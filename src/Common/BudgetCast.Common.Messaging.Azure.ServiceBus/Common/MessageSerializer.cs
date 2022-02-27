using System.Text.Json;
using BudgetCast.Common.Authentication;
using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Common;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Common;

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
        if (_identityContext.HasAssociatedTenant)
        {
            message.SetCurrentTenant(_identityContext.TenantId!.Value);

            _logger.LogInformationIfEnabled(
                "Tenant with id {TenantId} has been associated with message {MessageId}", 
                _identityContext.TenantId,
                message.Id);
        }

        if (_identityContext.HasAssociatedUser)
        {
            message.SetUserId(_identityContext.UserId);

            _logger.LogInformationIfEnabled(
                "User with id {UserId} has been associated with message {MessageId}", 
                _identityContext.UserId, 
                message.Id);
        }

        return JsonSerializer.Serialize(message, message.GetType());
    }

    public object? UnpackFromJson(string message, Type messageType) 
        => JsonSerializer.Deserialize(message, messageType);
}