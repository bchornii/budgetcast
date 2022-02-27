﻿using BudgetCast.Common.Authentication;
using BudgetCast.Common.Messaging.Abstractions.Events;

namespace BudgetCast.Notifications.AppHub.EventHandlers;

public class TestIntegrationEventHandler : IEventHandler<IntegrationEvent>
{
    private readonly IIdentityContext _identityContext;
    private readonly ILogger<TestIntegrationEventHandler> _logger;

    public TestIntegrationEventHandler(IIdentityContext identityContext, ILogger<TestIntegrationEventHandler> logger)
    {
        _identityContext = identityContext;
        _logger = logger;
    }
    
    public Task Handle(IntegrationEvent @event, CancellationToken cancellationToken)
    {
        var userId = _identityContext.UserId;
        var tenantId = _identityContext.TenantId;

        _logger.LogInformation("Received event: {@Event}", @event);
        
        return Task.CompletedTask;
    }
}