using BudgetCast.Dashboard.Compensations;
using Microsoft.AspNetCore.Http;

namespace BudgetCast.Dashboard.Api.Infrastructure.ExecutionHistoryStores
{
    public class ExecutionHistoryStore : IExecutionHistoryStore
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExecutionHistoryStore(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Add(string key, object value)
        {
            _httpContextAccessor.HttpContext.Items.Add(key, value);
        }

        public T Get<T>(string key) where T : class
        {
            return _httpContextAccessor.HttpContext
                .Items.TryGetValue(key, out var r) ? r as T : default(T);
        }
    }
}
