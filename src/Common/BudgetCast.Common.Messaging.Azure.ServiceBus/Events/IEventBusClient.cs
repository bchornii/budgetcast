using Azure.Messaging.ServiceBus;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Events;

public interface IEventBusClient
{
    ServiceBusClient Client { get; }
}