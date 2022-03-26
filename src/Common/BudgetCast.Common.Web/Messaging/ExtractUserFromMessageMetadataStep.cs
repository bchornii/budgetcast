using BudgetCast.Common.Authentication;
using BudgetCast.Common.Extensions;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using Microsoft.Extensions.Logging;
using static BudgetCast.Common.Web.Messaging.MessageMetadataConstants;

namespace BudgetCast.Common.Web.Messaging;

/// <summary>
/// Extracts user id from a received integration event and passes it
/// to <see cref="IIdentityContext"/>
/// </summary>
public class ExtractUserFromMessageMetadataStep :
    IMessagePreProcessingStep
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

    /// <inheritdoc/>
    public Task Execute(IntegrationMessage message, CancellationToken cancellationToken)
    {
        if (!_identityContext.HasAssociatedUser)
        {
            var userId = message.GetMetadata(UserIdMetadataKey);
                
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