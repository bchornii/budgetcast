namespace BudgetCast.Common.Domain
{
    public class AggregateRoot : Entity
    {
        public string CreatedBy { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public string? UpdatedBy { get; private set; }

        public DateTime? UpdatedAt { get; private set; }

        public AggregateRoot()
        {
            CreatedBy = default!;
            UpdatedBy = default!;
        }

        public void SetCreationDetails(string createdBy, DateTime createdAt)
        {
            CreatedBy = createdBy;
            CreatedAt = createdAt;
        }

        public void SetUpdateDetails(string updatedBy, DateTime updatedAt)
        {
            UpdatedBy = updatedBy;
            UpdatedAt = updatedAt;
        }
    }
}
