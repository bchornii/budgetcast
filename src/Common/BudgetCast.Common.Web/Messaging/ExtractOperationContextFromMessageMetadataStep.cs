using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using BudgetCast.Common.Operations;
using BudgetCast.Common.Web.Contextual;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Web.Messaging;

/// <summary>
/// Verifies if operation context is attached to the incoming message and if so, supplements
/// current operation information (aka. event handler) to the extracted from the message context
/// and saves it into <see cref="WorkloadContext"/> for further use.
/// </summary>
public class ExtractOperationContextFromMessageMetadataStep :
    IMessagePreProcessingStep
{
    private readonly WorkloadContext _workloadContext;
    private readonly ILogger<ExtractOperationContextFromMessageMetadataStep> _logger;

    public ExtractOperationContextFromMessageMetadataStep(
        WorkloadContext workloadContext,
        ILogger<ExtractOperationContextFromMessageMetadataStep> logger)
    {
        _workloadContext = workloadContext;
        _logger = logger;
    }
    
    public Task Execute(IntegrationMessage message, CancellationToken cancellationToken)
    {
        var wcDoesNotHaveOperationContext = !_workloadContext.Contains(OperationContext.MetaName);
        var messageHasOperationContext = message.HasMetadata(OperationContext.MetaName);
        
        if (messageHasOperationContext && wcDoesNotHaveOperationContext)
        {
            var messageOperationContext = message.GetMetadata(OperationContext.MetaName);
            var operationContext = OperationContext.Unpack(messageOperationContext);
            _logger.LogInformationIfEnabled(
                "Extracted operation context with {OperationContextId} id and '{OperationContextPath}' path from message {MessageId}", 
                operationContext.CorrelationId, operationContext.GetDescription(), message.Id);

            var messageName = message.GetMessageName();
            var currentOperationPart = new OperationPart($"{messageName}_Handler");
            _logger.LogInformationIfEnabled(
                "Updating operation context with {OperationContextId} id and '{OperationContextPath}' path extracted from message {MessageId} with '{OperationPart}' operation part",
                operationContext.CorrelationId, operationContext.GetDescription(), message.Id, currentOperationPart.Name);
            operationContext.Add(currentOperationPart);
            _logger.LogInformationIfEnabled(
                "Updated operation context with {OperationContextId} id and '{OperationContextPath}' path extracted from message {MessageId}", 
                operationContext.CorrelationId, operationContext.GetDescription(), message.Id);
                
            _workloadContext.AddItem(OperationContext.MetaName, operationContext);
            _logger.LogInformationIfEnabled(
                "Saved operation context with {OperationContextId} id and '{OperationContextPath}' path extracted from message {MessageId} into workload context", 
                operationContext.CorrelationId, operationContext.GetDescription(), message.Id);
        }

        return Task.CompletedTask;
    }
}