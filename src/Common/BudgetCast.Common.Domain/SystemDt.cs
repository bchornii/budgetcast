namespace BudgetCast.Common.Domain
{
    public interface ISystemDt
    {
        DateTime Current { get; }
    }

    public class SystemDt : ISystemDt
    {
        public DateTime Current => DateTime.UtcNow;
    }
}
