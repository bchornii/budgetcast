using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using BudgetCast.Common.Authentication;
using BudgetCast.Expenses.Messaging;
using BudgetCast.Notifications.AppHub.Application.NotifyExpenseAdded;
using BudgetCast.Notifications.AppHub.Models;
using BudgetCast.Notifications.AppHub.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static BudgetCast.Notifications.AppHub.Hubs.NotificationHub;

namespace BudgetCast.Notifications.Tests.Unit.Application.NotifyExpenseAdded;

public class NotifyExpenseAddedCommandHandlerTests
{
    private NotifyExpenseAddedCommandHandlerFixture _fixture;

    public NotifyExpenseAddedCommandHandlerTests()
    {
        _fixture = new NotifyExpenseAddedCommandHandlerFixture();
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
        
        var expectedGroupName = $"{TenantGroupPrefix}-{tenantId}";
        var expenseAddedEvent = _fixture.Fixture.Create<ExpensesAddedEvent>();
        
        // Act
        await _fixture.Handler.Handle(
            new NotifyExpenseAddedCommand(expenseAddedEvent), 
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
            .Be(NotificationMessageTypes.ExpensesAdded);
    }
    
    private class NotifyExpenseAddedCommandHandlerFixture
    {
        public Fixture Fixture { get; }
        
        public IIdentityContext IdentityContext { get; }
        
        public ILogger<NotifyExpenseAddedCommandHandler> Logger { get; }
        
        public INotificationService NotificationService { get; }

        public NotifyExpenseAddedCommandHandler Handler { get; }

        public NotifyExpenseAddedCommandHandlerFixture()
        {
            Fixture = new Fixture();
            IdentityContext = Mock.Of<IIdentityContext>();
            Logger = Mock.Of<ILogger<NotifyExpenseAddedCommandHandler>>();
            NotificationService = Mock.Of<INotificationService>();
            Handler = new NotifyExpenseAddedCommandHandler(
                IdentityContext,
                Logger,
                NotificationService,
                null!);
        }
    }
}