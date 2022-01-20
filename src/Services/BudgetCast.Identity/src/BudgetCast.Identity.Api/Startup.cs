using BudgetCast.Identity.Api.Infrastructure.Extensions;
using BudgetCast.Identity.Api.Infrastructure.Utils;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
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
                .AddAspNetIdentity(Configuration)
                .AddAuthentication(Configuration)
                .AddCustomHealthCheck(Configuration)
                .AddCustomHostedServices();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Budget Cast API");
            });

            app.UseSerilogRequestLogging(opts
                => opts.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest);

            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
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
