using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetCast.Common.Application.Tests.Unit.Stubs
{
    public class FakeData
    {
        public FakeData()
        {
            Notes = default!;
            FakeInnerData = default!;
            FakeInnerDatas = default!;
        }

        public int DealNumber { get; init; }

        public int DwellingAge { get; init; }

        public string Notes { get; init; }

        public FakeInnerData FakeInnerData { get; init; }

        public IReadOnlyCollection<FakeInnerData> FakeInnerDatas { get; init; }

        protected bool Equals(FakeData other)
        {
            var primitivePropertiesEqual =
                DealNumber == other.DealNumber &&
                DwellingAge == other.DwellingAge &&
                Notes == other.Notes;

            var complexPropertiesEqual =
                (FakeInnerData == null && other.FakeInnerData == null) || FakeInnerData.Equals(other.FakeInnerData);

            var collectionPropertiesEqual =
                (FakeInnerDatas == null && other.FakeInnerDatas == null) ||
                FakeInnerDatas.All(f => other.FakeInnerDatas.First(o => o.Id == f.Id).Equals(f));

            return primitivePropertiesEqual && complexPropertiesEqual && collectionPropertiesEqual;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((FakeData)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DealNumber, DwellingAge, Notes, FakeInnerData, FakeInnerDatas);
        }
    }
}
