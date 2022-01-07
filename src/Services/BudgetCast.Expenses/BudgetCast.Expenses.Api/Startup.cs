﻿using BudgetCast.Common.Web.Extensions;
using BudgetCast.Expenses.Api.Infrastructure.Extensions;
using BudgetCast.Expenses.Commands;
using BudgetCast.Expenses.Queries;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

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
                .AddControllers()
                .Services
                .AddHttpContextAccessor()
                .AddCustomSwagger()
                .AddDomain()
                .AddIdentityContext()
                .AddApplicationServices(
                    typeof(CommandsAssemblyMarkerType).Assembly,
                    typeof(QueriesAssemblyMarkerType).Assembly)
                .AddData(Configuration, Env)
                .AddCustomHealthCheck(Configuration)
                .AddCurrentTenant();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Budget Cast Expenses API");
            });

            app.UseRouting();
            app.UseCors();

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