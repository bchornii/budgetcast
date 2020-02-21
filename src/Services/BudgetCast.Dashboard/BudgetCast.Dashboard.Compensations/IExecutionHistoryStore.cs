namespace BudgetCast.Dashboard.Compensations
{
    public interface IExecutionHistoryStore
    {
        void Add(string key, object value);
        T Get<T>(string key) where T : class;
    }
}
