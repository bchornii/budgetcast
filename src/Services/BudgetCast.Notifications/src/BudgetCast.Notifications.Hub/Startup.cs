using BudgetCast.Common.Authentication;
using BudgetCast.Common.Web.Extensions;
using BudgetCast.Notifications.AppHub.Hubs;
using BudgetCast.Notifications.AppHub.Infrastructure.Extensions;
using BudgetCast.Notifications.AppHub.Models;
using BudgetCast.Notifications.AppHub.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace BudgetCast.Notifications.AppHub
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Env { get; }

        public Startup(
            IConfiguration configuration, 
            IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Env = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddCustomCors(Configuration)
                .AddCustomSignalR(Configuration)
                .AddCustomHealthCheck(Configuration)
                .AddJwtAuthentication(Configuration)
                .AddCustomServices()
                .AddIdentityContext()
                .AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseHttpsRedirection();
            
            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseCurrentTenant();
            app.UseAuthorization();

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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("/hubs/notifications", options =>
                {
                    //options.CloseOnAuthenticationExpiration = true;
                });

                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                    AllowCachingResponses = false
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self"),
                    AllowCachingResponses = false
                });
            });
        }
    }
}
