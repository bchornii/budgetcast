using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Common;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;

// TODO: 1) create az service bus resource
// TODO: 2) create budgetcast_events_topic topic
// TODO: 3) add TestEvent correlation filter based on Subject message property
// TODO: 4) send message into topic - debug sending + receiving

public static class HostExtensions
{
    /// <summary>
    /// Registers all types required to send and receive messages to/from
    /// Azure Service Bus.
    /// </summary>
    /// <param name="hostBuilder">Generic host builder</param>
    /// <param name="registerHandlers">Action to register message handlers in DI container</param>
    /// <param name="subscribeToEvents">Action to map events to their handlers</param>
    /// <param name="options">Azure Service Bus configuration parameters</param>
    /// <returns></returns>
    public static IHostBuilder UseAzureServiceBus(
        this IHostBuilder hostBuilder, 
        Action<IServiceCollection> registerHandlers,
        Action<IEventsProcessor> subscribeToEvents,
        Action<ServiceBusConfiguration>? options = null)
    {
        hostBuilder.ConfigureServices((ctx, services) =>
        {
            ServiceBusConfiguration? config = null;
            if (options is not null)
            {
                config = new ServiceBusConfiguration();
                options(config);   
            }

            // Common services
            services.AddScoped<IMessageSerializer, MessageSerializer>();
            
            // Event bus client
            services.AddSingleton<IEventBusClient, EventBusClient>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                return new EventBusClient(
                    connectionString: config?.AzureServiceBusConnectionString ?? 
                                      configuration["ServiceBusSettings:EventBusConnection"]);
            });

            // Sending message types
            services.AddScoped<IEventsPublisher, EventsPublisher>();

            // Processing message types
            services.AddSingleton<IEventsSubscriptionManager, EventsSubscriptionManager>();
            services.AddSingleton<IEventsProcessor, EventsProcessor>(provider =>
            {
                var subscriptionManager = provider.GetRequiredService<IEventsSubscriptionManager>();
                var logger = provider.GetRequiredService<ILogger<EventsProcessor>>();
                var configuration = provider.GetRequiredService<IConfiguration>();
                var processingPipeline = provider.GetRequiredService<IMessageProcessingPipeline>();
                var eventBusClient = provider.GetRequiredService<IEventBusClient>();
                var eventProcessor = new EventsProcessor(
                    subscriptionManager: subscriptionManager,
                    logger: logger,
                    subscriptionClientName: config?.SubscriptionClientName ?? 
                                            configuration["ServiceBusSettings:SubscriptionClientName"],
                    processingPipeline: processingPipeline,
                    eventBusClient: eventBusClient);

                subscribeToEvents(eventProcessor);
                
                return eventProcessor;
            });
            
            services.AddSingleton<IMessageProcessingPipeline, EventProcessingPipeline>();
            services.AddHostedService<EventsProcessorHostedService>();

            registerHandlers(services);
        });

        return hostBuilder;
    }

    /// <summary>
    /// Registers types required to send messages to Azure Service Bus.
    /// </summary>
    /// <param name="hostBuilder">Generic host builder</param>
    /// <param name="options">Azure Service Bus configuration parameters</param>
    /// <returns></returns>
    public static IHostBuilder UseAzureServiceBus(
        this IHostBuilder hostBuilder,
        Action<ServiceBusConfiguration>? options = null)
    {
        hostBuilder.ConfigureServices((ctx, services) =>
        {
            ServiceBusConfiguration? config = null;
            if (options is not null)
            {
                config = new ServiceBusConfiguration();
                options(config);
            }

            services.AddSingleton<IEventBusClient, EventBusClient>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                return new EventBusClient(
                    connectionString: config?.AzureServiceBusConnectionString ??
                                      configuration["ServiceBusSettings:EventBusConnection"]);
            });

            services.AddScoped<IEventsPublisher, EventsPublisher>();
            services.AddScoped<IMessageSerializer, MessageSerializer>();
        });

        return hostBuilder;
    }
}