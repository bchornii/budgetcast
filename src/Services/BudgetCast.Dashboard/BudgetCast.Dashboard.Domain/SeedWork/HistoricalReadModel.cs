using System;

namespace BudgetCast.Dashboard.Domain.SeedWork
{
    public class HistoricalReadModel : IdentifiableReadModel
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}