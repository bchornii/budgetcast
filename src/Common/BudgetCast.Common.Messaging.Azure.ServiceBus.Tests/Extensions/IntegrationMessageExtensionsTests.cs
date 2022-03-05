using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;
using FluentAssertions;
using Xunit;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Tests.Extensions;

public class IntegrationMessageExtensionsTests
{
    [Fact]
    public void GetMessageName_Message_Is_Not_Passed_Returns_Fallback_MessageName()
    {
        // Arrange
        
        // Act
        var result = IntegrationMessageExtensions.GetMessageName(null);

        // Assert
        result
            .Should()
            .Be(IntegrationMessageExtensions.FallBackMessageName);
    }
    
    [Fact]
    public void GetMessageName_Message_Is_Passed_Returns_MessageName()
    {
        // Arrange
        
        // Act
        var result = new FakeIntegrationMessage().GetMessageName();

        // Assert
        result
            .Should()
            .Be(nameof(FakeIntegrationMessage));
    }

    private class FakeIntegrationMessage : IntegrationMessage
    {
    }
}