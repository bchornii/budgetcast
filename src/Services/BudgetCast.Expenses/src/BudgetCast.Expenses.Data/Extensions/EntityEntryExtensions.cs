using BudgetCast.Common.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BudgetCast.Expenses.Data.Extensions
{
    public static class EntityEntryExtensions
    {
        public static void UpdateEntityAuditableValues(
            this IEnumerable<EntityEntry<IAuditableEntity>> entities,
            DateTime now,
            string currentUserId)
        {
            foreach (var entry in entities)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = currentUserId;
                        entry.Entity.LastModifiedBy = currentUserId;
                        entry.Entity.CreatedOn = now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = now;
                        entry.Entity.LastModifiedBy = currentUserId;
                        break;

                    case EntityState.Deleted:
                        if (entry.Entity is ISoftDelete softDelete)
                        {
                            softDelete.DeletedBy = currentUserId;
                            softDelete.DeletedOn = now;
                            entry.State = EntityState.Modified;
                        }
                        break;
                }
            }
        }

        public static void UpdateTenantValues(
            this IEnumerable<EntityEntry<IMustHaveTenant>> entities, 
            long TenantId)
        {
            foreach (var entry in entities)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Modified:
                        if (entry.Entity.TenantId == default)
                        {
                            entry.Entity.TenantId = TenantId;
                        }

                        break;
                }
            }
        }
    }
}
