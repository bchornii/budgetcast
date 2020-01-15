using System;

namespace BudgetCast.Dashboard.Domain.SeedWork
{
    public class AggregateRoot : Entity
    {
        private string _createdBy;
        private DateTime _createdAt;
        private string _updatedBy;
        private DateTime _updatedAt;
    }
}