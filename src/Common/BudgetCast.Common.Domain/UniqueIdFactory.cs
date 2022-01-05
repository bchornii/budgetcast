namespace BudgetCast.Common.Domain
{
    public class UniqueIdFactory
    {
        public static string GetId() => Guid.NewGuid().ToString("N");
    }
}
