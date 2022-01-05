namespace BudgetCast.Common.Domain
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
    }
}
