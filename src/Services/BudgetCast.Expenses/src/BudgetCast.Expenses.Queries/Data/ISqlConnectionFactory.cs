using System.Data;

namespace BudgetCast.Expenses.Queries.Data
{
    public interface ISqlConnectionFactory
    {
        Task<IDbConnection> GetOpenConnection(CancellationToken cancellationToken);
    }
}
