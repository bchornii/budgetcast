using BudgetCast.Common.Authentication;
using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Common;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Common;

public class ExtractTenantFromMessageMetadataStep :
    IMessagePreProcessingStep
{
    private readonly IIdentityContext _identityContext;
    private readonly ILogger<ExtractTenantFromMessageMetadataStep> _logger;

    public ExtractTenantFromMessageMetadataStep(
        IIdentityContext identityContext,
        ILogger<ExtractTenantFromMessageMetadataStep> logger)
    {
        _identityContext = identityContext;
        _logger = logger;
    }

    public Task Execute(IntegrationMessage message, CancellationToken cancellationToken)
    {
        if (!_identityContext.HasAssociatedTenant)
        {
            var tenantId = message.GetTenantId();
            _logger.LogInformationIfEnabled(
                "Extracted {TenantId} tenant id from message {MessageId}", tenantId, message.Id);
                
            if (tenantId.HasValue)
            {
                _identityContext.SetCurrentTenant(tenantId.Value);
            }
        }

        return Task.CompletedTask;
    }
}