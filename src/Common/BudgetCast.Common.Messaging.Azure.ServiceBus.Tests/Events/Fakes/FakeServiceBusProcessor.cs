using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events.Fakes;

internal class FakeServiceBusProcessor : ServiceBusProcessor
{
    public bool WasProcessingStarted { get; private set; }

    public bool WasProcessingStopped { get; private set; }
    
    public bool WasErrorCallbackCalled { get; private set; }
    
    public bool WasProcessingCallbackCalled { get; private set; }

    public override Task StartProcessingAsync(
        CancellationToken cancellationToken = new())
    {
        WasProcessingStarted = true;
        return Task.CompletedTask;
    }

    public override Task StopProcessingAsync(
        CancellationToken cancellationToken = new())
    {
        WasProcessingStopped = true;
        return Task.CompletedTask;
    }

    public async Task<FakeProcessMessageEventArgs> SimulateMessageReceivingOf(
        string messageId, 
        string subject, 
        string payload, 
        int attempt = 1)
    {
        var args = CreateMessageArgs(messageId, subject, payload, attempt);
        await OnProcessMessageAsync(args);
        return (FakeProcessMessageEventArgs)args;
    }
    
    public async Task<FakeProcessMessageEventArgs> SimulateMessageReceivingOf(
        string messageId, 
        string subject, 
        string payload, 
        bool rethrowExceptions)
    {
        var args = CreateMessageArgs(messageId, subject, payload);
        try
        {
            await OnProcessMessageAsync(args);
            return (FakeProcessMessageEventArgs)args;
        }
        catch (Exception e)
        {
            if (rethrowExceptions)
            {
                throw;   
            }

            return (FakeProcessMessageEventArgs)args;
        }
    }

    public async Task SimulateErrorReceiving()
    {
        await OnProcessErrorAsync(new ProcessErrorEventArgs(
            new Exception(),
            ServiceBusErrorSource.Receive,
            string.Empty,
            string.Empty,
            CancellationToken.None));
    }

    protected override Task OnProcessMessageAsync(ProcessMessageEventArgs args)
    {
        WasProcessingCallbackCalled = true;
        return base.OnProcessMessageAsync(args);
    }

    protected override Task OnProcessErrorAsync(ProcessErrorEventArgs args)
    {
        WasErrorCallbackCalled = true;
        return base.OnProcessErrorAsync(args);
    }

    private ProcessMessageEventArgs CreateMessageArgs(
        string messageId, 
        string subject, 
        string payload, 
        int deliveryCount = 1)
    {
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: BinaryData.FromString(payload),
            subject: subject,
            messageId: messageId,
            deliveryCount: deliveryCount);

        return new FakeProcessMessageEventArgs(message);
    }
}