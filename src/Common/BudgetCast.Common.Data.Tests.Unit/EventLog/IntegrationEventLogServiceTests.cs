using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Common.Application.Outbox;
using BudgetCast.Common.Data.EventLog;
using BudgetCast.Common.Data.Tests.Unit.Fakes;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using Xunit;

namespace BudgetCast.Common.Data.Tests.Unit.EventLog;

public class IntegrationEventLogServiceTests
{
    private IntegrationEventLogServiceFixture _fixture;

    public IntegrationEventLogServiceTests()
    {
        _fixture = new IntegrationEventLogServiceFixture();
    }

    [Fact]
    public async Task RetrieveScopedEventsPendingToPublishAsync_Returns_Only_Scoped_NonPublished_EventEntries()
    {
        // Arrange
        _fixture
            .AddScopedEventEntries(5, EventStateEnum.NotPublished)
            .AddScopedEventEntries(5, EventStateEnum.Published)
            .AddScopedEventEntries(5, EventStateEnum.InProgress)
            .AddScopedEventEntries(5, EventStateEnum.PublishedFailed)
            .AddNonScopedEventEntries(5, EventStateEnum.NotPublished);

        // Act
        var result = (await _fixture.EventLogService
            .RetrieveScopedEventsPendingToPublishAsync()).ToArray();

        // Assert
        result.Length.Should().Be(5);
        
        result
            .Select(r => r.ScopeId)
            .Should()
            .AllBe(_fixture.EventLogService.ScopeId);
    }

    [Fact]
    public async Task AddEventAsync_Should_Add_Entry_To_IntegrationEventLogs()
    {
        // Arrange
        var initialRecords = _fixture.OperationalDbContext.IntegrationEventLogs.Count();
        var integrationEvent = new FakeIntegrationEvent();
        
        // Act
        await _fixture.EventLogService.AddEventAsync(integrationEvent, CancellationToken.None);
        await _fixture.OperationalDbContext.SaveChangesAsync(CancellationToken.None);

        // Assert
        var resultingTotal = _fixture.OperationalDbContext.IntegrationEventLogs.Count();

        resultingTotal.Should().Be(initialRecords + 1);
    }

    [Fact]
    public async Task MarkEventAsPublishedAsync_Should_Mark_Event_AsPublished()
    {
        // Arrange
        var integrationEvent = new FakeIntegrationEvent();
        await _fixture.EventLogService.AddEventAsync(integrationEvent, CancellationToken.None);
        await _fixture.OperationalDbContext.SaveChangesAsync(CancellationToken.None);
        
        // Act
        await _fixture.EventLogService.MarkEventAsPublishedAsync(integrationEvent.Id);

        // Assert
        var eventEntry = _fixture.OperationalDbContext
            .IntegrationEventLogs.First(e => e.EventId == integrationEvent.Id);

        eventEntry.State.Should().Be(EventStateEnum.Published);
    }
    
    [Fact]
    public async Task MarkEventAsInProgressAsync_Should_Mark_Event_AsInProgress()
    {
        // Arrange
        var integrationEvent = new FakeIntegrationEvent();
        await _fixture.EventLogService.AddEventAsync(integrationEvent, CancellationToken.None);
        await _fixture.OperationalDbContext.SaveChangesAsync(CancellationToken.None);
        
        // Act
        await _fixture.EventLogService.MarkEventAsInProgressAsync(integrationEvent.Id);

        // Assert
        var eventEntry = _fixture.OperationalDbContext
            .IntegrationEventLogs.First(e => e.EventId == integrationEvent.Id);

        eventEntry.State.Should().Be(EventStateEnum.InProgress);
    }
    
    [Fact]
    public async Task MarkEventAsInProgressAsync_Called_2_Times_Should_Increment_TimesSent_Counter()
    {
        // Arrange
        var integrationEvent = new FakeIntegrationEvent();
        await _fixture.EventLogService.AddEventAsync(integrationEvent, CancellationToken.None);
        await _fixture.OperationalDbContext.SaveChangesAsync(CancellationToken.None);
        
        // Act
        await _fixture.EventLogService.MarkEventAsInProgressAsync(integrationEvent.Id);
        await _fixture.EventLogService.MarkEventAsInProgressAsync(integrationEvent.Id);

        // Assert
        var eventEntry = _fixture.OperationalDbContext
            .IntegrationEventLogs.First(e => e.EventId == integrationEvent.Id);

        eventEntry.TimesSent.Should().Be(2);
    }
    
    [Fact]
    public async Task MarkEventAsInProgressAsync_Should_Mark_Event_AsPublishFailed()
    {
        // Arrange
        var integrationEvent = new FakeIntegrationEvent();
        await _fixture.EventLogService.AddEventAsync(integrationEvent, CancellationToken.None);
        await _fixture.OperationalDbContext.SaveChangesAsync(CancellationToken.None);
        
        // Act
        await _fixture.EventLogService.MarkEventAsFailedAsync(integrationEvent.Id);

        // Assert
        var eventEntry = _fixture.OperationalDbContext
            .IntegrationEventLogs.First(e => e.EventId == integrationEvent.Id);

        eventEntry.State.Should().Be(EventStateEnum.PublishedFailed);
    }

    private class IntegrationEventLogServiceFixture
    {
        public OperationalDbContext OperationalDbContext { get; }
        
        public IntegrationEventLogService EventLogService { get; }

        private DbContextOptions<OperationalDbContext> ContextOptions { get; }

        public IntegrationEventLogServiceFixture()
        {
            ContextOptions = new DbContextOptionsBuilder<OperationalDbContext>()
                .UseInMemoryDatabase("ExpensesDb").Options;
            OperationalDbContext = new OperationalDbContext(ContextOptions);
            OperationalDbContext.Database.EnsureDeleted();
            OperationalDbContext.Database.EnsureCreated();
            
            EventLogService = new IntegrationEventLogService(
                OperationalDbContext,
                () => new List<Type>
                {
                    typeof(FakeIntegrationEvent)
                });
        }

        public IntegrationEventLogServiceFixture AddNonScopedEventEntries(int total, EventStateEnum state)
        {
            for (int i = 0; i < total; i++)
            {
                var entry = GetLogEntry(string.Empty, state);
                OperationalDbContext.IntegrationEventLogs.Add(entry);
            }
            OperationalDbContext.SaveChanges();
            return this;
        }

        public IntegrationEventLogServiceFixture AddScopedEventEntries(int total, EventStateEnum state)
        {
            var scopeId = EventLogService.ScopeId;
            for (var i = 0; i < total; i++)
            {
                var entry = GetLogEntry(scopeId, state);
                OperationalDbContext.IntegrationEventLogs.Add(entry);
            }
            OperationalDbContext.SaveChanges();
            return this;
        }

        private static IntegrationEventLogEntry GetLogEntry(string scopeId, EventStateEnum state = EventStateEnum.NotPublished)
            => new IntegrationEventLogEntry(new FakeIntegrationEvent(), scopeId)
            {
                State = state
            };
    }
}