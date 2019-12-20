using BudgetCast.Dashboard.Api.AppSettings;
using BudgetCast.Dashboard.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;

namespace BudgetCast.Dashboard.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Env { get; }

        public Startup(IConfiguration configuration,
            IHostingEnvironment environment)
        {
            Configuration = configuration;
            Env = environment;
        }        

        public void ConfigureServices(IServiceCollection services)
        {
            if (Env.IsDevelopment())
            {
                services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy", builder =>
                        builder
                            .WithOrigins("http://localhost:4200")
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials());
                });
            }            

            services.AddMvc()
                .AddFluentValidation(options =>
                {
                    options.RegisterValidatorsFromAssemblyContaining<Startup>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Budget Cast API", Version = "v1" });
            });

            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseSqlServer(Configuration["IdentityManagement:ConnectionString"],
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(
                            typeof(Startup).GetTypeInfo().Assembly.GetName().Name);

                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 15,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            });

            services
                .AddIdentity<IdentityUser, IdentityRole>(options =>
                 {
                     options.Password.RequiredLength = 8;
                     options.Password.RequireNonAlphanumeric = false;
                     options.Password.RequireLowercase = true;
                     options.Password.RequireUppercase = false;
                     options.Password.RequireDigit = true;

                     options.User.RequireUniqueEmail = true;
                 })
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<EmailParameters>(
                Configuration.GetSection("EmailParameters"));
            services.AddScoped<EmailService>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;

                options.Events.OnRedirectToLogin = redirectContext =>
                {
                    redirectContext.HttpContext.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToAccessDenied = redirectContext =>
                {
                    redirectContext.HttpContext.Response.StatusCode = 403;
                    return Task.CompletedTask;
                };
            });

            services.Configure<UiLinks>(
                Configuration.GetSection("UiLinks"));

            services.Configure<ExternalIdentityProviders>(
                Configuration.GetSection("ExternalIdentityProviders"));

            services
                .AddAuthentication(options =>
                {
                    options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
                })
                .AddGoogle(Configuration["ExternalIdentityProviders:Google:Name"],
                    options =>
                    {
                        options.CallbackPath = new PathString("/g-callback");
                        options.ClientId = Configuration["Social:Google:ClientId"];
                        options.ClientSecret = Configuration["Social:Google:ClientSecret"];
                    })
                .AddFacebook(Configuration["ExternalIdentityProviders:Facebook:Name"],
                    options =>
                    {
                        options.CallbackPath = new PathString("/fb-callback");
                        options.ClientId = Configuration["Social:Facebook:ClientId"];
                        options.ClientSecret = Configuration["Social:Facebook:ClientSecret"];
                    });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors("CorsPolicy");
            }
            
            if(!Env.IsDevelopment())
            {
                app.UseDefaultFiles();
                app.UseStaticFiles();
            }            

            app.UseHttpsRedirection();

            if (Env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Budget Cast API");
                });
            }

            app.UseAuthentication();
            app.UseMvc();
            app.UseSpa(_ => { });
        }
    }
}
