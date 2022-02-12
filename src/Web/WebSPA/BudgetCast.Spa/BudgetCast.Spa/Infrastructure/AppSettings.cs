namespace BudgetCast.Spa.Infrastructure
{
    public class AppSettings
    {
        public Endpoints Endpoints { get; set; }
    }
    
    public class Endpoints
    {
        public string Identity { get; set; }

        public string Expenses { get; set; }

        public string Notifications { get; set; }
    }
}
