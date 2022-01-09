using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

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

        public static ModelBuilder AppendGlobalQueryFilter<TInterface>(this ModelBuilder modelBuilder, Expression<Func<TInterface, bool>> expression)
        {
            // gets a list of entities that implement the interface TInterface
            var entities = modelBuilder.Model
                .GetEntityTypes()
                .Where(e => e.ClrType.GetInterface(typeof(TInterface).Name) != null)
                .Select(e => e.ClrType);
            foreach (var entity in entities)
            {
                var parameterType = Expression.Parameter(modelBuilder.Entity(entity).Metadata.ClrType);
                var expressionFilter = ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), parameterType, expression.Body);

                // get existing query filters of the entity(s)
                var currentQueryFilter = modelBuilder.Entity(entity).GetQueryFilter();
                if (currentQueryFilter != null)
                {
                    var currentExpressionFilter = ReplacingExpressionVisitor.Replace(currentQueryFilter.Parameters.Single(), parameterType, currentQueryFilter.Body);

                    // Append new query filter with the existing filter
                    expressionFilter = Expression.AndAlso(currentExpressionFilter, expressionFilter);
                }

                var lambdaExpression = Expression.Lambda(expressionFilter, parameterType);

                // applies the filter to the entity(s)
                modelBuilder.Entity(entity).HasQueryFilter(lambdaExpression);
            }

            return modelBuilder;
        }

        private static LambdaExpression? GetQueryFilter(this EntityTypeBuilder builder)
        {
            return builder?.Metadata?.GetQueryFilter();
        }
    }    
}
