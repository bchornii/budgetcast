using BudgetCast.Common.Domain;
using BudgetCast.Expenses.Api.Infrastructure.AppSettings;
using BudgetCast.Expenses.Data;
using BudgetCast.Expenses.Data.Campaigns;
using BudgetCast.Expenses.Data.Expenses;
using BudgetCast.Expenses.Domain.Campaigns;
using BudgetCast.Expenses.Domain.Expenses;
using BudgetCast.Expenses.Queries.Campaigns;
using BudgetCast.Expenses.Queries.Expenses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using BudgetCast.Common.Data;
using BudgetCast.Common.Data.EventLog;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Web.Filters;
using BudgetCast.Expenses.Commands;
using BudgetCast.Expenses.Messaging;

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

            services.AddControllers(
                options => options.Filters.Add<OperationFilter>());

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Budget Cast Expenses API", Version = "v1" });

                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

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

            services.AddScoped<OperationalDbContext>(services =>
                services.GetRequiredService<ExpensesDbContext>());

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            services.AddScoped<IIntegrationEventLogService>(services => new IntegrationEventLogService(
                services.GetRequiredService<OperationalDbContext>(), () =>
                {
                    var integrationEventType = typeof(IntegrationEvent);
                    return typeof(ExpensesAddedEvent)
                        .Assembly
                        .GetTypes()
                        .Where(t => t.IsAssignableTo(integrationEventType))
                        .ToList();
                }));

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

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection($"SecuritySettings:{nameof(JwtSettings)}").Get<JwtSettings>();

            if (string.IsNullOrEmpty(jwtSettings.Key))
            {
                throw new InvalidOperationException("No Key defined in JwtSettings config.");
            }

            services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key)),

                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,

                        RequireExpirationTime = true,
                        ValidateLifetime = true,

                        RequireSignedTokens = true,
                        RequireAudience = true,

                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.Name,

                        ClockSkew = TimeSpan.FromMinutes(5) //5 minute tolerance for the expiration date                        
                    };
                });

            return services;
        }
    }
}
