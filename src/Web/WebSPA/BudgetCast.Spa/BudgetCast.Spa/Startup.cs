using System;
using BudgetCast.Spa.Infrastructure;
using BudgetCast.Spa.Infrastructure.ExternalUtils;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BudgetCast.Spa
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Uri UiRootUri { get; }

        public string UiRootUrl { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            UiRootUri = new Uri(Configuration.GetValue<string>("UiBaseUrl"));
            UiRootUrl = UiRootUri.OriginalString;
        }        

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration);

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                    builder
                        .WithOrigins(UiRootUrl)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());

            // Setup where the compiled version of our spa application will be, when in production. 
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAntiforgery antiforgery)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();

            // Here we add Angular default Antiforgery cookie name on first load. https://angular.io/guide/http#security-xsrf-protection
            // This cookie will be read by Angular app and its value will be sent back to the application as the header configured in .AddAntiforgery()
            app.Use(next => context =>
            {
                string path = context.Request.Path.Value;

                if (
                    string.Equals(path, "/", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(path, "/index.html", StringComparison.OrdinalIgnoreCase))
                {
                    // The request token has to be sent as a JavaScript-readable cookie, 
                    // and Angular uses it by default.
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
                        new CookieOptions() { HttpOnly = false });
                }

                return next(context);
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSerilogRequestLogging(opts
                => opts.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest);

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self"),
                    AllowCachingResponses = false
                });
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                    AllowCachingResponses = false
                });
            });

            // Handles all still unnatended (by any other middleware) requests by returning the default page of the SPA (wwwroot/index.html).
            app.UseSpa(spa =>
            {
                // the root of the angular app. (Where the package.json lives)
                spa.Options.SourcePath = "Client";
                spa.Options.DevServerPort = UiRootUri.Port;

                if (env.IsDevelopment())
                {

                    // use the SpaServices extension method for angular, that will make the application to run "ng serve" for us, when in development.
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
