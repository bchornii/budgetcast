namespace BudgetCast.Common.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T>? source) 
            => source ?? Array.Empty<T>();
    }
}
