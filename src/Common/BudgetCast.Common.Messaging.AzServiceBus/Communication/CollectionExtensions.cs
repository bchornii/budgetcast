﻿namespace BudgetCast.Common.Messaging.AzServiceBus.Communication
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T>? source) 
            => source ?? Array.Empty<T>();
    }
}
