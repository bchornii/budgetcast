using BudgetCast.Common.Authentication;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.AzServiceBus.Extensions;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Messaging.AzServiceBus.Common.PreHandling
{
    public class ExtractTenantFromMessageMetadataStep :
        IMessagePreHandlingStep
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
}
