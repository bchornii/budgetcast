using Azure.Messaging.ServiceBus;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Events;

/// <summary>
/// Represents an abstraction over <see cref="ServiceBusClient"/> type.
/// </summary>
public interface IEventBusClient
{
    ServiceBusClient Client { get; }
}