using BudgetCast.Common.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BudgetCast.Expenses.Data.Converters
{
    public static class UserTypeConverters
    {
        public static ValueConverter GetTypedIdConverter()
        {
            return new ValueConverter<TypedId, ulong>(
                v => v.Value,
                v => new TypedId(v));
        }
    }
}
