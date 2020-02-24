namespace BudgetCast.Dashboard.Compensations
{
    public interface IExecutionHistoryStore
    {
        void Add(string key, object value = null);
        T Get<T>(string key) where T : class;
        bool Exists(string key);
    }
}
