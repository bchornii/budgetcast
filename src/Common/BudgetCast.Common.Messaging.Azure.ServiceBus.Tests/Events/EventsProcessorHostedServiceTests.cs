using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Events;
using Moq;
using Xunit;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Events;

public class EventsProcessorHostedServiceTests
{
    private readonly EventsProcessorHostedServiceFixture _fixture;

    public EventsProcessorHostedServiceTests()
    {
        _fixture = new EventsProcessorHostedServiceFixture();
    }
    
    [Fact]
    public async Task StartAsync_Should_Start_MessageProcessing()
    {
        // Arrange
        
        // Act
        await _fixture.Service.StartAsync(CancellationToken.None);

        // Assert
        Mock.Get(_fixture.EventsProcessor)
            .Verify(v => v.Start(CancellationToken.None));
    }
    
    [Fact]
    public async Task StopAsync_Should_Stop_MessageProcessing()
    {
        // Arrange
        
        // Act
        await _fixture.Service.StopAsync(CancellationToken.None);

        // Assert
        Mock.Get(_fixture.EventsProcessor)
            .Verify(v => v.Stop(CancellationToken.None));
    }
    
    private class EventsProcessorHostedServiceFixture
    {
        public IEventsProcessor EventsProcessor { get; }
        
        public EventsProcessorHostedService Service { get; }

        public EventsProcessorHostedServiceFixture()
        {
            EventsProcessor = Mock.Of<IEventsProcessor>();
            Service = new EventsProcessorHostedService(EventsProcessor);
        }
    }
}