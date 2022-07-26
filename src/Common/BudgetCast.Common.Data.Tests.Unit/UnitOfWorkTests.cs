using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using BudgetCast.Common.Application.Outbox;
using BudgetCast.Common.Data.Tests.Unit.Fakes;
using BudgetCast.Common.Domain;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Expenses.Commands;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BudgetCast.Common.Data.Tests.Unit;

public class UnitOfWorkTests
{
    private readonly UnitOfWorkFixture _fixture;

    public UnitOfWorkTests()
    {
        _fixture = new UnitOfWorkFixture()
            .InitializeDefaultStubs();
    }

    [Fact]
    public async Task Commit_ExistingTransactionIsInProgress_ShouldReturn_True_Immediately()
    {
        // Arrange
        Mock.Get(_fixture.OperationalDbContext)
            .Setup(s => s.HasActiveTransaction)
            .Returns(true);

        // Act
        var result = await _fixture.UnitOfWork.Commit(CancellationToken.None);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public async Task Commit_Should_Dispatch_Domain_Events()
    {
        // Arrange

        // Act
        await _fixture.UnitOfWork.Commit(CancellationToken.None);

        // Assert
        Mock.Get(_fixture.DomainEventsDispatcher)
            .Verify(v => v.DispatchEventsAsync(CancellationToken.None));
    }
    
    [Fact]
    public async Task Commit_Should_Execute_And_Wrap_SaveChanges_In_Transaction()
    {
        // Arrange
        var transaction = Mock.Of<IDbContextTransaction>();
        Mock.Get(_fixture.OperationalDbContext)
            .Setup(s => s.BeginTransactionAsync(CancellationToken.None))
            .ReturnsAsync(transaction);
        
        // Act
        await _fixture.UnitOfWork.Commit(CancellationToken.None);

        // Assert
        Mock.Get(_fixture.OperationalDbContext)
            .Verify(v => v.SaveChangesAsync(CancellationToken.None));
        
        Mock.Get(_fixture.OperationalDbContext)
            .Verify(v => v.CommitTransactionAsync(transaction, CancellationToken.None));
    }

    [Fact]
    public async Task Commit_Should_Publish_Pending_IntegrationEvents()
    {
        // Arrange
        var totalEventLogEntries = 5;
        var pendingEvents = _fixture.GetEventLogEntries(totalEventLogEntries);

        Mock.Get(_fixture.EventLogService)
            .Setup(s => s.RetrieveScopedEventsPendingToPublishAsync())
            .ReturnsAsync(pendingEvents);

        // Act
        await _fixture.UnitOfWork.Commit(CancellationToken.None);

        // Assert
        Mock.Get(_fixture.EventLogService)
            .Verify(v => v.MarkEventAsInProgressAsync(It.IsAny<string>()), Times.Exactly(totalEventLogEntries));
            
        Mock.Get(_fixture.EventsPublisher)
            .Verify(v => v.Publish(It.IsAny<IntegrationEvent>(), CancellationToken.None), Times.Exactly(totalEventLogEntries));
        
        Mock.Get(_fixture.EventLogService)
            .Verify(v => v.MarkEventAsPublishedAsync(It.IsAny<string>()), Times.Exactly(totalEventLogEntries));
    }
    
    private class UnitOfWorkFixture
    {
        private ILogger<UnitOfWork> Logger { get; }
        
        public OperationalDbContext OperationalDbContext { get; }
        
        public IIntegrationEventLogService EventLogService { get; }
        
        public IEventsPublisher EventsPublisher { get; }
        
        public IDomainEventsDispatcher DomainEventsDispatcher { get; }
        
        public Fixture Fixture { get; }

        public UnitOfWork UnitOfWork { get; }

        public UnitOfWorkFixture()
        {
            var options = new DbContextOptions<OperationalDbContext>();
            OperationalDbContext = new Mock<OperationalDbContext>(options).Object;
            Fixture = new Fixture();
            Logger = Mock.Of<ILogger<UnitOfWork>>();
            EventLogService = Mock.Of<IIntegrationEventLogService>();
            EventsPublisher = Mock.Of<IEventsPublisher>();
            DomainEventsDispatcher = Mock.Of<IDomainEventsDispatcher>();
            UnitOfWork = new UnitOfWork(OperationalDbContext, Logger, EventLogService, EventsPublisher, DomainEventsDispatcher);
        }

        public UnitOfWorkFixture InitializeDefaultStubs()
        {
            var database = new Mock<DatabaseFacade>(OperationalDbContext).Object;
            var executionStrategy = new ExecutionStrategy();
            Mock.Get(database)
                .Setup(s => s.CreateExecutionStrategy())
                .Returns(executionStrategy);
        
            Mock.Get(OperationalDbContext)
                .Setup(s => s.Database)
                .Returns(database);
            
            Mock.Get(OperationalDbContext)
                .Setup(s => s.BeginTransactionAsync(CancellationToken.None))
                .ReturnsAsync(Mock.Of<IDbContextTransaction>());
            
            return this;
        }
        
        public IReadOnlyCollection<IntegrationEventLogEntry> GetEventLogEntries(int total) 
            => Enumerable.Repeat(new IntegrationEventLogEntry(new FakeIntegrationEvent(), string.Empty), total).ToArray();

        private class ExecutionStrategy : IExecutionStrategy
        {
            public TResult Execute<TState, TResult>(
                TState state, 
                Func<DbContext, TState, TResult> operation, 
                Func<DbContext, TState, ExecutionResult<TResult>>? verifySucceeded)
            {
                var context = state as DbContext;
                return operation(context, state);
            }

            public async Task<TResult> ExecuteAsync<TState, TResult>(
                TState state, 
                Func<DbContext, TState, CancellationToken, Task<TResult>> operation, 
                Func<DbContext, TState, CancellationToken, Task<ExecutionResult<TResult>>>? verifySucceeded,
                CancellationToken cancellationToken = new())
            {
                var context = state as DbContext;
                return await operation(context, state, cancellationToken);
            }

            public bool RetriesOnFailure { get; } = false;
        }
    }
}