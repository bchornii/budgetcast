namespace BudgetCast.Common.Domain
{
    public class TypedId : IEquatable<TypedId>
    {
        public ulong Value { get; }

        public TypedId(ulong value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TypedId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(TypedId other)
        {
            return Value == other.Value;
        }

        public static bool operator ==(TypedId obj1, TypedId obj2)
        {
            if (Equals(obj1, null))
            {
                if (Equals(obj2, null))
                {
                    return true;
                }
                return false;
            }
            return obj1.Equals(obj2);
        }
        public static bool operator !=(TypedId x, TypedId y)
        {
            return !(x == y);
        }

        public static implicit operator TypedId(ulong value)
            => new TypedId(value);

        public static implicit operator ulong(TypedId typedId)
            => typedId.Value;
    }
}
