using BudgetCast.Common.Messaging.Abstractions.Common;
using BudgetCast.Common.Messaging.Abstractions.Events;

namespace BudgetCast.Common.Messaging.Azure.ServiceBus.Extensions;

public static class IntegrationMessageExtensions
{
    public const string FallBackMessageName = "UndeterminedMessageName";
    
    public static string GetMessageName(this IntegrationMessage? message)
        => message?.GetType().Name ?? FallBackMessageName;
}