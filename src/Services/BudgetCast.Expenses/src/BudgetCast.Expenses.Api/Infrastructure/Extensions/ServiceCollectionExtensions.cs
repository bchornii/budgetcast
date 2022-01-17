using BudgetCast.Common.Domain;
using BudgetCast.Expenses.Data;
using BudgetCast.Expenses.Data.Campaigns;
using BudgetCast.Expenses.Data.Expenses;
using BudgetCast.Expenses.Domain.Campaigns;
using BudgetCast.Expenses.Domain.Expenses;
using BudgetCast.Expenses.Queries.Campaigns;
using BudgetCast.Expenses.Queries.Expenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace BudgetCast.Expenses.Api.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomMvc(this IServiceCollection services,
            IConfiguration configuration)
        {
            var uiRoot = configuration["UiLinks:Root"];
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                    builder
                        .WithOrigins(uiRoot)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddControllers();

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Budget Cast Expenses API", Version = "v1" });
            });

            return services;
        }

        public static IServiceCollection AddCustomControllers(
            this IServiceCollection services)
        {
            services.AddControllers();

            return services;
        }

        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            hcBuilder
                .AddSqlServer(
                    configuration["ExpensesDb:ConnectionString"],
                    name: "Expenses-SqlDb-Check",
                    tags: new string[] { "expensesdb" });

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services,
            IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddDbContext<ExpensesDbContext>(options =>
            {
                options.UseSqlServer(configuration["ExpensesDb:ConnectionString"],
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 15,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });

                if (env.IsDevelopment())
                {
                    options.LogTo(Console.WriteLine, LogLevel.Information, DbContextLoggerOptions.LocalTime);
                    options.EnableSensitiveDataLogging();
                }
            });

            services.AddScoped<IUnitOfWork>(services =>
                    services.GetRequiredService<ExpensesDbContext>());

            return services;
        }

        public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
        {
            services.AddScoped<ICampaignDataAccess, CampaignDataAccess>();
            services.AddScoped<IExpensesDataAccess, ExpensesDataAccess>();
            return services;
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<ICampaignRepository, CampaignRepository>();
            services.AddScoped<IExpensesRepository, ExpensesRepository>();
            return services;
        }
    }
}
