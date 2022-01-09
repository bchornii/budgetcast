using System;

namespace BudgetCast.Common.Application.Tests.Unit.Stubs
{
    public class FakeInnerData
    {
        public int Id { get; init; }

        public string Name { get; init; }

        public string Description { get; init; }

        public FakeInnerData()
        {
            Name = default!;
            Description = default!;
        }

        protected bool Equals(FakeInnerData other)
        {
            return Id == other.Id && Name == other.Name && Description == other.Description;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FakeInnerData)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Description);
        }
    }
}
