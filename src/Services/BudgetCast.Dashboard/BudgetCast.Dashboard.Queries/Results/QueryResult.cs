using System.Collections.Generic;

namespace BudgetCast.Dashboard.Queries.Results
{
    public enum ResultStatus
    {
        Success,
        NotFound,
        BadQuery
    }

    public class QueryResult<T>
    {
        public QueryResult()
        {
            Messages = new List<string>();
        }

        public ResultStatus Status { get; private set; }

        public T Value { get; private set; }

        public List<string> Messages { get; }

        public bool IsSuccess() => Status == ResultStatus.Success;

        public bool NotFound() => Status == ResultStatus.NotFound;

        public static QueryResult<T> GetSuccessResult(T value)
        {
            return new QueryResult<T>
            {
                Value = value,
                Status = ResultStatus.Success
            };
        }

        public static QueryResult<T> GetNotFoundResult(string message = null)
        {
            return new QueryResult<T>
            {
                Status = ResultStatus.NotFound,
                Messages = { message }
            };
        }

        public static QueryResult<T> GetBadQueryResult(string message = null)
        {
            return new QueryResult<T>
            {
                Status = ResultStatus.BadQuery,
                Messages = { message }
            };
        }
    }
}
