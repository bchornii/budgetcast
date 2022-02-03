using BudgetCast.Common.Authentication;
using BudgetCast.Notifications.AppHub.Models;
using BudgetCast.Notifications.AppHub.Services;

namespace BudgetCast.Notifications.AppHub.Middlewares
{
    public static class ApplicationBuilerExtensions
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
                        notification: new BasicNotification
                        {
                            Label = NotificationType.Success,
                            Message = $"Hi, group {groupName}",
                            MessageType = nameof(BasicNotification),
                        },
                        @group: groupName, cancellationToken: CancellationToken.None);

                    await ctx.Response.WriteAsync("Message sent");
                }
                else if (ctx.Request.Path.StartsWithSegments("/api/test-user"))
                {
                    var userId = identityCtx.UserId;
                    await notificationService.SendMessageToUserAsync(userId: userId, notification: new BasicNotification
                    {
                        Label = NotificationType.Success,
                        Message = $"Hi, {userId}",
                        MessageType = nameof(BasicNotification),
                    }, cancellationToken: CancellationToken.None);

                    await ctx.Response.WriteAsync("Message sent");
                }
                else
                {
                    await next();
                }
            });

            return app;
        }
    }
}
