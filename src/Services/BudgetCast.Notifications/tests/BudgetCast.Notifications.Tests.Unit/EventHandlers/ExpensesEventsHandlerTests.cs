using System;
using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Expenses.Messaging;
using BudgetCast.Notifications.AppHub.Application.NotifyExpenseAdded;
using BudgetCast.Notifications.AppHub.Application.NotifyExpenseRemoved;
using BudgetCast.Notifications.AppHub.EventHandlers;
using MediatR;
using Moq;
using Xunit;

namespace BudgetCast.Notifications.Tests.Unit.EventHandlers;

public class ExpensesEventsHandlerTests
{
    private ExpensesEventsHandlerFixture _fixture;

    public ExpensesEventsHandlerTests()
    {
        _fixture = new ExpensesEventsHandlerFixture();
    }

    [Fact]
    public async Task Handle_ExpensesAddedEvent_Should_Send_NotifyExpenseAddedCommand()
    {
        // Arrange
        
        // Act
        await _fixture.Handler.Handle(
            new ExpensesAddedEvent(
                It.IsAny<long>(),
                It.IsAny<long>(),
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>()), 
            CancellationToken.None);

        // Assert
        Mock.Get(_fixture.Mediator)
            .Verify(v => v.Send(
                It.IsAny<NotifyExpenseAddedCommand>(), 
                CancellationToken.None));
    }
    
    [Fact]
    public async Task Handle_ExpensesRemovedEvent_Should_Send_NotifyExpenseRemovedCommand()
    {
        // Arrange
        
        // Act
        await _fixture.Handler.Handle(
            new ExpensesRemovedEvent(
                It.IsAny<long>(),
                It.IsAny<long>(),
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<string>()), 
            CancellationToken.None);

        // Assert
        Mock.Get(_fixture.Mediator)
            .Verify(v => v.Send(
                It.IsAny<NotifyExpenseRemovedCommand>(), 
                CancellationToken.None));
    }
    
    private class ExpensesEventsHandlerFixture
    {
        public IMediator Mediator { get; }
        
        public ExpensesEventsHandler Handler { get; }

        public ExpensesEventsHandlerFixture()
        {
            Mediator = Mock.Of<IMediator>();
            Handler = new ExpensesEventsHandler(Mediator);
        }
    }
}