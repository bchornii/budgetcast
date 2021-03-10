using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;

namespace WebStatus
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());

            var maximumHistoryEntriesPerEndpoint = Convert.ToInt32(
                Configuration["MaximumHistoryEntriesPerEndpoint"]);           
            services
                .AddHealthChecksUI(setup =>
                {
                    setup.MaximumHistoryEntriesPerEndpoint(maximumHistoryEntriesPerEndpoint);
                    setup.UseApiEndpointHttpMessageHandler(_ =>
                    {
                        return new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = (__, ___, ____, _____) => true
                        };
                    });
                })
                .AddInMemoryStorage();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHealthChecksUI(config =>
                {
                    config.UIPath = "/hc-ui";                    
                });

                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }
}