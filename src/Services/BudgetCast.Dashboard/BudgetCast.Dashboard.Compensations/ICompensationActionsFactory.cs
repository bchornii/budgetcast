namespace BudgetCast.Dashboard.Compensations
{
    public interface ICompensationActionsFactory
    {
        bool TryGet(string routeName, out ICompensationAction action);
    }
}
