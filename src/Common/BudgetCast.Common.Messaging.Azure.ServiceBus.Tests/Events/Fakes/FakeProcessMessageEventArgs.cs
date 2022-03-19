using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events.Fakes;

public class FakeProcessMessageEventArgs : ProcessMessageEventArgs
{
    public bool WasCompleted { get; private set; }
    
    public bool WasDeadLettered { get; private set; }
    
    public FakeProcessMessageEventArgs(ServiceBusReceivedMessage message) 
        : base(message, null!, CancellationToken.None)
    {
    }

    public override Task CompleteMessageAsync(
        ServiceBusReceivedMessage message,
        CancellationToken cancellationToken = new())
    {
        WasCompleted = true;
        return Task.CompletedTask;
    }

    public override Task DeadLetterMessageAsync(
        ServiceBusReceivedMessage message, 
        string deadLetterReason,
        string? deadLetterErrorDescription = null, 
        CancellationToken cancellationToken = new())
    {
        WasDeadLettered = true;
        return Task.CompletedTask;
    }

    public override Task DeadLetterMessageAsync(
        ServiceBusReceivedMessage message, 
        IDictionary<string, object>? propertiesToModify = null,
        CancellationToken cancellationToken = new())
    {
        WasDeadLettered = true;
        return Task.CompletedTask;
    }
}