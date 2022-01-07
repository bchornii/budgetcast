using BudgetCast.Common.Authentication;
using BudgetCast.Common.Domain;
using BudgetCast.Common.Web.Middleware;
using BudgetCast.Expenses.Data.Extensions;
using BudgetCast.Expenses.Domain.Campaigns;
using BudgetCast.Expenses.Domain.Expenses;
using Microsoft.EntityFrameworkCore;

namespace BudgetCast.Expenses.Data
{
    public class ExpensesDbContext : DbContext, IUnitOfWork
    {
        public const string DbSchema = "dbo";
        private readonly IIdentityContext _identityContext;

        public DbSet<Expense> Expenses { get; set; }

        public DbSet<Expense> ExpenseItems { get; set; }

        public DbSet<Campaign> Campaigns { get; set; }

        public long Tenant { get; set; }

        public ExpensesDbContext(
            DbContextOptions<ExpensesDbContext> options, 
            IIdentityContext identityContext, 
            ITenantService tenantService)
            : base(options)
        {
            Tenant = tenantService.TenantId;

            _identityContext = identityContext;
            Expenses = Set<Expense>();
            ExpenseItems = Set<Expense>();
            Campaigns = Set<Campaign>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfigurationsFromAssembly(typeof(ExpensesDbContext).Assembly)
                .MarkDateTimeColumnsAsDateTimeInDb();

            modelBuilder
                .AppendGlobalQueryFilter<IMustHaveTenant>(b => b.TenantId == Tenant)
                .AppendGlobalQueryFilter<ISoftDelete>(s => s.DeletedOn == null);
        }

        public async Task<bool> Commit()
        {
            var currentUserId = _identityContext.UserId;
            var now = SystemDt.Current;
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
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

            foreach (var entry in ChangeTracker.Entries<IMustHaveTenant>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                    case EntityState.Modified:
                        if (entry.Entity.TenantId == default)
                        {
                            entry.Entity.TenantId = Tenant;
                        }

                        break;
                }
            }

            var result = await base.SaveChangesAsync();
            return result > 0;
        }
    }
}
