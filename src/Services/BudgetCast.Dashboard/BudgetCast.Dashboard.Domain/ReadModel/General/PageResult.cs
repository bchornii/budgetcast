using System.Collections.Generic;

namespace BudgetCast.Dashboard.Domain.ReadModel.General
{
    public class PageResult<T>
    {
        public IReadOnlyList<T> Items { get; }
        public long Total { get; }

        public PageResult(IReadOnlyList<T> items, long total)
        {
            Items = items;
            Total = total;
        }
    }
}
