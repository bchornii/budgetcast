using System;

namespace BudgetCast.Dashboard.Compensations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CompensationActionAttribute : Attribute
    {
        public string Name { get; set; }
    }
}