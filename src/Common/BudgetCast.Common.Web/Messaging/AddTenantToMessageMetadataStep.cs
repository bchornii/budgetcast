using BudgetCast.Common.Authentication;
using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using Microsoft.Extensions.Logging;
using static BudgetCast.Common.Web.Messaging.MessageMetadataConstants;

namespace BudgetCast.Common.Web.Messaging;

/// <summary>
/// Adds tenant information to integration message metadata
/// </summary>
public class AddTenantToMessageMetadataStep : IMessagePreSendingStep
{
    private readonly IIdentityContext _identityContext;
    private readonly ILogger<AddTenantToMessageMetadataStep> _logger;

    public AddTenantToMessageMetadataStep(
        IIdentityContext identityContext, 
        ILogger<AddTenantToMessageMetadataStep> logger)
    {
        _identityContext = identityContext;
        _logger = logger;
    }
    
    public Task Execute(IntegrationMessage message, CancellationToken cancellationToken)
    {
        var messageName = message.GetMessageName(); 
        if (_identityContext.HasAssociatedTenant && !message.HasMetadata(TenantIdMetadataKey))
        {
            message.SetMetadata(TenantIdMetadataKey, _identityContext.TenantId!.Value.ToString());

            _logger.LogInformationIfEnabled("TenantId {TenantId} has been added to {MessageName} message with id {MessageId}", 
                _identityContext.TenantId, messageName, message.Id);
        }
        
        return Task.CompletedTask;
    }
}