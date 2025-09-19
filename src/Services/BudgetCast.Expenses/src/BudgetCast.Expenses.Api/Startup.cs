using BudgetCast.Common.Application.Extensions;
using BudgetCast.Common.Data.OperationRegistry;
using BudgetCast.Common.Web.Extensions;
using BudgetCast.Common.Web.Logs;
using BudgetCast.Expenses.Api.Infrastructure.Extensions;
using BudgetCast.Expenses.Commands;
using BudgetCast.Expenses.Queries;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;

namespace BudgetCast.Expenses.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Env = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddCustomMvc(Configuration)
                .AddHostedServices(Configuration)
                .AddJwtAuthentication(Configuration)
                .AddHttpContextAccessor()
                .AddCustomSwagger()
                .AddIdentityContext()
                .AddDomainServices()
                .AddDataAccessServices()
                .AddApplicationServices(
                    operationRegistryType: typeof(MsSqlOperationsRegistry),
                    typeof(CommandsAssemblyMarkerType).Assembly,
                    typeof(QueriesAssemblyMarkerType).Assembly)
                .AddCustomDbContext(Configuration, Env)
                .AddCustomHealthCheck(Configuration)
                .AddMessagingExtensions()
                .AddOperationContext()
                .AddApplicationInsightsTelemetry(options =>
                {
                    options.ConnectionString = Configuration
                        .GetValue<string>("BudgetCast:ApplicationInsights:ConnectionString");
                })
                .AddHttpLogging(options =>
                {
                    options.LoggingFields = HttpLoggingFields.All;
                    options.RequestHeaders.Add("X-Correlation-ID");
                    options.ResponseHeaders.Add("WWW-Authenticate");
                    options.RequestBodyLogLimit = 4096;
                    options.ResponseBodyLogLimit = 4096;
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseApiExceptionHandling(isDevelopment: false);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Budget Cast Expenses API");
            });

            app.UseRouting();
            app.UseCors();

            app.UseHttpLogging();
            app.UseSharedSerilogRequestLogging();
            
            app.UseAuthentication();
            app.UseCurrentTenant();
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
