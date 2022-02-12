using BudgetCast.Common.Authentication;
using BudgetCast.Common.Web.Extensions;
using BudgetCast.Notifications.AppHub.Hubs;
using BudgetCast.Notifications.AppHub.Infrastructure.Extensions;
using BudgetCast.Notifications.AppHub.Middlewares;
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
            else
            {
                app.UseApiExceptionHandling(
                    isDevelopment: Env.IsDevelopment());
            }
            
            app.UseHttpsRedirection();
            //app.UseHttpLogging();
            
            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseCurrentTenant();
            app.UseAuthorization();

            if (Env.IsDevelopment())
            {
                app.UseTestEndpoints();
            }            

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
