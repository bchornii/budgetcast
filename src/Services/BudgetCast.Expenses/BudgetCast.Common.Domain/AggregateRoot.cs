namespace BudgetCast.Common.Domain
{
    public class AggregateRoot : Entity
    {
        private string _createdBy;
        private DateTime _createdAt;
        private string _updatedBy;
        private DateTime _updatedAt;

        public AggregateRoot()
        {
            _createdBy = default!;
            _updatedBy = default!;
        }
    }
}
