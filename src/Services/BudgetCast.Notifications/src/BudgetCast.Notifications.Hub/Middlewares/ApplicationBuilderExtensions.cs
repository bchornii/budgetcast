using BudgetCast.Common.Authentication;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Expenses.Messaging;
using BudgetCast.Notifications.AppHub.EventHandlers;
using BudgetCast.Notifications.AppHub.Models;
using BudgetCast.Notifications.AppHub.Services;

namespace BudgetCast.Notifications.AppHub.Middlewares;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseTestEndpoints(this IApplicationBuilder app)
    {
        app.Use(async (ctx, next) =>
        {
            var notificationService = ctx.RequestServices
                .GetRequiredService<INotificationService>();

            var identityCtx = ctx.RequestServices
                .GetRequiredService<IIdentityContext>();

            if (ctx.Request.Path.StartsWithSegments("/api/test-group"))
            {
                var groupName = $"GroupTenant-{identityCtx.TenantId}";
                await notificationService.SendMessageToGroupAsync(
                    notification: new GeneralNotification
                    {
                        Type = NotificationType.Success,
                        Message = $"Hi, group {groupName}",
                    },
                    @group: groupName, cancellationToken: CancellationToken.None);

                await ctx.Response.WriteAsync("Message sent");
            }
            else if (ctx.Request.Path.StartsWithSegments("/api/test-group-add-expenses"))
            {
                var groupName = $"GroupTenant-{identityCtx.TenantId}";
                await notificationService.SendMessageToGroupAsync(
                    notification: new GeneralNotification
                    {
                        Type = NotificationType.Success,
                        Message = $"Hi, group {groupName}",
                        MessageType = NotificationMessageTypes.ExpensesAdded,
                        Data = new
                        {
                            Id = Guid.NewGuid(),
                            Total = 1023.0,
                            AddedBy = "bchornii",
                            AddedAt = DateTime.Now
                        }
                    },
                    @group: groupName, cancellationToken: CancellationToken.None);

                await ctx.Response.WriteAsync("Message sent");
            }
            else if (ctx.Request.Path.StartsWithSegments("/api/test-group-remove-expenses"))
            {
                var groupName = $"GroupTenant-{identityCtx.TenantId}";
                await notificationService.SendMessageToGroupAsync(
                    notification: new GeneralNotification
                    {
                        Type = NotificationType.Success,
                        Message = $"Hi, group {groupName}",
                        MessageType = NotificationMessageTypes.ExpensesRemoved,
                        Data = new
                        {
                            RemovedBy = "bchornii",
                            RemovedAt = DateTime.Now
                        }
                    },
                    @group: groupName, cancellationToken: CancellationToken.None);

                await ctx.Response.WriteAsync("Message sent");
            }
            else if (ctx.Request.Path.StartsWithSegments("/api/test-user"))
            {
                var userId = identityCtx.UserId;
                await notificationService.SendMessageToUserAsync(
                    userId: userId,
                    notification: new GeneralNotification
                    {
                        Type = NotificationType.Success,
                        Message = $"Hi, {userId}",
                    }, cancellationToken: CancellationToken.None);

                await ctx.Response.WriteAsync("Message sent");
            }
            else if (ctx.Request.Path.StartsWithSegments("/api/test-event"))
            {
                var eventPublisher = ctx.RequestServices
                    .GetRequiredService<IEventsPublisher>();

                var id = Guid.NewGuid().ToString();
                await eventPublisher
                    .Publish(new ExpenseAddedEventFake(id), CancellationToken.None);
            }
            else
            {
                await next();
            }
        });

        return app;
    }
}

internal class ExpenseAddedEventFake : ExpensesAddedEvent
{
    public ExpenseAddedEventFake(string id) 
        : base(7024, 1221, 100.0m, "bchornii", DateTime.UtcNow, "TestCampaign")
    {
        Id = id;
    }
}