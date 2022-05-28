using BudgetCast.Common.Web.Extensions;
using BudgetCast.Common.Web.Logs;
using BudgetCast.Identity.Api.Infrastructure.AppSettings;
using BudgetCast.Identity.Api.Infrastructure.Extensions;
using BudgetCast.Identity.Api.Infrastructure.Utils;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;

namespace BudgetCast.Identity.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Env { get; }

        public Startup(IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Env = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSwagger()
                .AddCustomConfigSections(Configuration)
                .AddCustomServices()
                .AddCustomMvc(Configuration)
                .AddIdentity(Configuration)
                .AddLocalization()
                .AddAuthorization()
                .AddJwtAuthentication(Configuration)
                .AddCustomHealthCheck(Configuration)
                .AddCustomHostedServices()
                .AddIdentityContext()
                .AddApplicationInsightsTelemetry(options =>
                {
                    options.ConnectionString = Configuration
                        .GetValue<string>("BudgetCast:ApplicationInsights:ConnectionString");
                })
                .AddHttpLogging(options =>
                {
                    options.LoggingFields = HttpLoggingFields.All;
                    options.RequestHeaders.Add("X-Correlation-ID");
                    options.RequestBodyLogLimit = 4096;
                    options.ResponseBodyLogLimit = 4096;
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseApiExceptionHandling(isDevelopment: false);

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Budget Cast API");
            });

            app.UseRouting();
            app.UseCors();

            app.UseHttpLogging();
            app.UseSharedSerilogRequestLogging();
            
            app.UseAuthentication();
            app.UseCurrentTenant(pathsToExlude:
                TenantConfiguration.PathsToExcludeFromTenantVerification);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

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
