using System.Diagnostics.CodeAnalysis;
using Azure.Messaging.ServiceBus;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Events;

[ExcludeFromCodeCoverage]
public class EventBusClient : IAsyncDisposable
{
    private static readonly object Token = new();
 
    private ServiceBusClient _client;
    private readonly string _connectionString;

    public virtual ServiceBusClient Client
    {
        get
        {
            if (!_client.IsClosed)
            {
                return _client;
            }
            
            lock (Token)
            {
                if (_client.IsClosed)
                {
                    _client = new ServiceBusClient(_connectionString);   
                }
            }
            return _client;
        }
    }

    public EventBusClient(string connectionString)
    {
        _connectionString = connectionString;
        _client = new ServiceBusClient(_connectionString);
    }

    public async ValueTask DisposeAsync() 
        => await Client.DisposeAsync();
}