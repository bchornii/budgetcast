using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using BudgetCast.Common.Operations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Web.Messaging;

/// <summary>
/// Adds or updates if already existing in metadata <see cref="OperationContext"/>
/// before sending a message.
/// </summary>
public class AddOrUpdateOperationContextStep : IMessagePreSendingStep
{
    public const string InitialOperationSuffix = "Initial_Operation";
    
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AddOrUpdateOperationContextStep> _logger;

    public AddOrUpdateOperationContextStep(
        IServiceProvider serviceProvider, 
        ILogger<AddOrUpdateOperationContextStep> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    public Task Execute(IntegrationMessage message, CancellationToken cancellationToken)
    {
        const string stepName = nameof(AddOrUpdateOperationContextStep);
        var messageName = message.GetMessageName();
        
        var parentOperationContext = _serviceProvider
            .GetRequiredService<OperationContext>();
        
        if (parentOperationContext.IsEmpty)
        {
            var isOperationContextSetOnMessage = message
                .HasMetadata(OperationContext.MetaName);

            if (isOperationContextSetOnMessage)
            {
                _logger.LogInformationIfEnabled(
                    "Parent operation context has not been set but {MessageName} message with id {MessageId} has operation context in the metadata", 
                    messageName, message.Id);
                
                return Task.CompletedTask;
            }
        
            parentOperationContext.Add(new OperationPart($"{messageName}_{InitialOperationSuffix}"));
            
            var operationContextPacked = parentOperationContext.Pack();
            message.SetMetadata(OperationContext.MetaName, operationContextPacked);
            
            _logger.LogInformationIfEnabled(
                "New operation context with {OperationContextId} and {OperationContextPath} has been initialized & added to {MessageName} message with id {MessageId}", 
                parentOperationContext.CorrelationId, parentOperationContext.GetDescription(), messageName, message.Id);
        }
        else
        {
            var operationContextPacked = parentOperationContext.Pack();
            message.SetMetadata(OperationContext.MetaName, operationContextPacked);
            
            _logger.LogInformationIfEnabled(
                "Parent operation context with {OperationContextId} and {OperationContextPath} has been added to {MessageName} message with id {MessageId}", 
                parentOperationContext.CorrelationId, parentOperationContext.GetDescription(), messageName, message.Id);
        }
        
        
        return Task.CompletedTask;
    }
}