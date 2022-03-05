using System;
using System.Collections.Generic;
using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Abstractions.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Common;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Events;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Extensions;

public class HostExtensionsTests
{
    private HostExtensionsFixture _fixture;

    public HostExtensionsTests()
    {
        _fixture = new HostExtensionsFixture();
    }

    [Fact]
    public void UseAzureServiceBus_SendOnly_Should_Add_EventsPublisher_AsScoped()
    {
        // Arrange

        // Act
        _fixture.HostBuilder.UseAzureServiceBus();

        // Assert
        Mock.Get(_fixture.ServiceCollection)
            .Verify(v => v.Add(It.Is<ServiceDescriptor>(it => 
                it.ImplementationType == typeof(EventsPublisher) &&
                it.ServiceType == typeof(IEventsPublisher) &&
                it.Lifetime == ServiceLifetime.Scoped)));
    }
    
    [Fact]
    public void UseAzureServiceBus_SendOnly_Should_Add_MessageSerializer_AsScoped()
    {
        // Arrange

        // Act
        _fixture.HostBuilder.UseAzureServiceBus();

        // Assert
        Mock.Get(_fixture.ServiceCollection)
            .Verify(v => v.Add(It.Is<ServiceDescriptor>(it => 
                it.ImplementationType == typeof(MessageSerializer) &&
                it.ServiceType == typeof(IMessageSerializer) &&
                it.Lifetime == ServiceLifetime.Scoped)));
    }
    
    [Fact]
    public void UseAzureServiceBus_SendOnly_Should_Add_EventBusClient_AsSingleton()
    {
        // Arrange

        // Act
        _fixture.HostBuilder.UseAzureServiceBus();

        // Assert
        Mock.Get(_fixture.ServiceCollection)
            .Verify(v => v.Add(It.Is<ServiceDescriptor>(it => 
                it.ServiceType == typeof(EventBusClient) &&
                it.Lifetime == ServiceLifetime.Singleton)));
    }
    
    private class HostExtensionsFixture
    {
        public IHostBuilder HostBuilder { get; }
        
        public HostBuilderContext HostBuilderContext { get; }

        public IServiceCollection ServiceCollection { get; }

        public HostExtensionsFixture()
        {
            HostBuilderContext = new HostBuilderContext(new Dictionary<object, object>());
            ServiceCollection = Mock.Of<IServiceCollection>();
            HostBuilder = new FakeHostBuilder(HostBuilderContext, ServiceCollection);
        }

        public class FakeHostBuilder : IHostBuilder
        {
            private HostBuilderContext _context;
            private IServiceCollection _collection;
            
            public FakeHostBuilder(HostBuilderContext context, IServiceCollection collection)
            {
                _context = context;
                _collection = collection;
                Properties = new Dictionary<object, object>();
            }
            
            public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
            {
                throw new NotImplementedException();
            }

            public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
            {
                return this;
            }

            public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
            {
                configureDelegate(_context, _collection);
                return this;
            }

            public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
            {
                return this;
            }

            public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
            {
                return this;
            }

            public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
            {
                return this;
            }

            public IHost Build()
            {
                return null!;
            }

            public IDictionary<object, object> Properties { get; }
        }
    }
}