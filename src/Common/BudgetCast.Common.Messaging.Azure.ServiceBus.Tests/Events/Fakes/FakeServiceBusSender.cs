using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events.Fakes;

internal class FakeServiceBusSender : ServiceBusSender
{
    public ServiceBusMessage CachedMessage { get; private set; }
            
    public override Task SendMessageAsync(
        ServiceBusMessage message, 
        CancellationToken cancellationToken = new())
    {
        CachedMessage = message;
        return Task.CompletedTask;
    }
}