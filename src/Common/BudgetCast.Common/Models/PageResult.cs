namespace BudgetCast.Common.Models
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
