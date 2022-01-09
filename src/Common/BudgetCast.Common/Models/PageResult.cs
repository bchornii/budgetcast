namespace BudgetCast.Common.Models
{
    public class PageResult<T>
    {
        public IReadOnlyList<T> Items { get; }

        public int CurrentPage { get; }

        public int TotalPages { get; }

        public int TotalCount { get; }

        public int PageSize { get; }

        public PageResult(IReadOnlyList<T> items, int pageSize, int pageNumber, int count)
        {
            Items = items;
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
