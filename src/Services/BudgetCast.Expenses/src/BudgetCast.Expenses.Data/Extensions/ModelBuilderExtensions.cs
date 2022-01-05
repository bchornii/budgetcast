using Microsoft.EntityFrameworkCore;

namespace BudgetCast.Expenses.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder MarkDateTimeColumnsAsDateTimeInDb(this ModelBuilder builder)
        {
            var dateProperties = from e in builder.Model.GetEntityTypes()
                                 from p in e.GetProperties()
                                 where p.ClrType == typeof(DateTime) ||
                                       p.ClrType == typeof(DateTime?)
                                 select p;

            foreach (var property in dateProperties)
            {
                property.SetColumnType("DATETIME");
            }

            return builder;
        }
    }
}
