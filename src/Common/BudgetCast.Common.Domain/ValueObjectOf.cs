namespace BudgetCast.Common.Domain;

public abstract class ValueObjectOf<T>
    where T : ValueObjectOf<T>
{
    protected abstract bool EqualsCore(T other);
    protected abstract int GetHashCodeCore();

    public override bool Equals(object obj)
    {
        var valueObj = obj as T;

        if (ReferenceEquals(valueObj, null))
        {
            return false;
        }
        return EqualsCore(valueObj);
    }

    public override int GetHashCode()
    {
        return GetHashCodeCore();
    }

    public static bool operator ==(ValueObjectOf<T> left, ValueObjectOf<T> right)
    {
        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(ValueObjectOf<T> left, ValueObjectOf<T> right)
    {
        return !(left == right);
    }
}