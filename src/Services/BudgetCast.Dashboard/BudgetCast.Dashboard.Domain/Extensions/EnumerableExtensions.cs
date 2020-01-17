using System.Collections.Generic;
using System.Linq;

namespace BudgetCast.Dashboard.Domain.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Next<T>(this IEnumerable<T> source, int amountOfItems)
        {
            if (source == null)
            {
                return Enumerable.Empty<IEnumerable<T>>();
            }

            return source
                .Select((item, inx) => new { item, inx })
                .GroupBy(x => x.inx / amountOfItems)
                .Select(g => g.Select(x => x.item));
        }

        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source?.Any() ?? true;
        }
    }
}
