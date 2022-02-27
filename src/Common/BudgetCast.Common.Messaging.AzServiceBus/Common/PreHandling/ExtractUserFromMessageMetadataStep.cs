using BudgetCast.Common.Authentication;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.AzServiceBus.Extensions;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Messaging.AzServiceBus.Common.PreHandling
{
    public class ExtractUserFromMessageMetadataStep :
        IMessagePreHandlingStep
    {
        private readonly IIdentityContext _identityContext;
        private readonly ILogger<ExtractUserFromMessageMetadataStep> _logger;

        public ExtractUserFromMessageMetadataStep(
            IIdentityContext identityContext,
            ILogger<ExtractUserFromMessageMetadataStep> logger)
        {
            _identityContext = identityContext;
            _logger = logger;
        }

        public Task Execute(IntegrationMessage message, CancellationToken cancellationToken)
        {
            if (!_identityContext.HasAssociatedUser)
            {
                var userId = message.GetUserId();
                
                _logger.LogInformationIfEnabled(
                    "Extracted {UserId} user id from message {MessageId}", userId, message.Id);
                
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    _identityContext.SetUserId(userId);
                }
            }

            return Task.CompletedTask;
        }
    }
}
