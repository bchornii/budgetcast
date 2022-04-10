using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using BudgetCast.Common.Authentication;
using BudgetCast.Common.Operations;
using BudgetCast.Expenses.Messaging;
using BudgetCast.Notifications.AppHub.Application.NotifyExpenseRemoved;
using BudgetCast.Notifications.AppHub.Hubs;
using BudgetCast.Notifications.AppHub.Models;
using BudgetCast.Notifications.AppHub.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BudgetCast.Notifications.Tests.Unit.Application.NotifyExpenseRemoved;

public class NotifyExpenseRemovedCommandHandlerTests
{
    private NotifyExpenseRemovedCommandHandlerFixture _fixture;

    public NotifyExpenseRemovedCommandHandlerTests()
    {
        _fixture = new NotifyExpenseRemovedCommandHandlerFixture();
    }

    [Fact]
    public async Task Handle_Should_Send_Notification_With_CorrectData()
    {
        // Arrange
        var tenantId = _fixture.Fixture.Create<long>();

        Mock.Get(_fixture.IdentityContext)
            .Setup(s => s.TenantId)
            .Returns(tenantId);

        GeneralNotification cachedNotification = null!;
        var cachedGroup = string.Empty;
        Mock.Get(_fixture.NotificationService)
            .Setup(s => s.SendMessageToGroupAsync(
                It.IsAny<INotificationMessage>(), 
                It.IsAny<string>(),
                CancellationToken.None))
            .Callback<INotificationMessage, string, CancellationToken>(((message, group, _) =>
            {
                cachedGroup = group;
                cachedNotification = (GeneralNotification)message;
            }))
            .Returns(Task.CompletedTask);
        
        var expectedGroupName = $"{NotificationHub.TenantGroupPrefix}-{tenantId}";
        var expenseRemovedEvent = _fixture.Fixture.Create<ExpensesRemovedEvent>();
        
        // Act
        await _fixture.Handler.Handle(
            new NotifyExpenseRemovedCommand(expenseRemovedEvent), 
            CancellationToken.None);
        
        // Assert
        cachedGroup.Should().Be(expectedGroupName);

        cachedNotification
            .Type
            .Should()
            .Be(NotificationType.Success);

        cachedNotification
            .MessageType
            .Should()
            .Be(NotificationMessageTypes.ExpensesRemoved);
    }
    
    private class NotifyExpenseRemovedCommandHandlerFixture
    {
        private OperationContext OperationContext { get; }
        
        private ILogger<NotifyExpenseRemovedCommandHandler> Logger { get; }
        
        public Fixture Fixture { get; }

        public IIdentityContext IdentityContext { get; }

        public INotificationService NotificationService { get; }

        public NotifyExpenseRemovedCommandHandler Handler { get; }

        public NotifyExpenseRemovedCommandHandlerFixture()
        {
            Fixture = new Fixture();
            IdentityContext = Mock.Of<IIdentityContext>();
            Logger = Mock.Of<ILogger<NotifyExpenseRemovedCommandHandler>>();
            NotificationService = Mock.Of<INotificationService>();
            OperationContext = OperationContext.New();
            Handler = new NotifyExpenseRemovedCommandHandler(
                IdentityContext,
                Logger,
                NotificationService,
                OperationContext);
        }
    }
}