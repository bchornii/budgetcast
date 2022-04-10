using BudgetCast.Common.Authentication;
using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using Microsoft.Extensions.Logging;
using static BudgetCast.Common.Web.Messaging.MessageMetadataConstants;

namespace BudgetCast.Common.Web.Messaging;

/// <summary>
/// Adds user information to integration message metadata
/// </summary>
public class AddUserToMessageMetadataStep : IMessagePreSendingStep
{
    private readonly IIdentityContext _identityContext;
    private readonly ILogger<AddUserToMessageMetadataStep> _logger;

    public AddUserToMessageMetadataStep(
        IIdentityContext identityContext, 
        ILogger<AddUserToMessageMetadataStep> logger)
    {
        _identityContext = identityContext;
        _logger = logger;
    }
    
    public Task Execute(IntegrationMessage message, CancellationToken cancellationToken)
    {
        var messageName = message.GetMessageName();

        if (_identityContext.HasAssociatedUser && !message.HasMetadata(UserIdMetadataKey))
        {
            message.SetMetadata(UserIdMetadataKey, _identityContext.UserId);

            _logger.LogInformationIfEnabled("UserId {UserId} has been added to {MessageName} message with id {MessageId}", 
                _identityContext.UserId, messageName, message.Id);
        }
        
        return Task.CompletedTask;
    }
}