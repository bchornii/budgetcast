using BudgetCast.Common.Authentication;
using BudgetCast.Common.Data;
using BudgetCast.Common.Domain;
using BudgetCast.Expenses.Data.Extensions;
using BudgetCast.Expenses.Domain.Campaigns;
using BudgetCast.Expenses.Domain.Expenses;
using BudgetCast.Expenses.Queries.Campaigns;
using Microsoft.EntityFrameworkCore;

namespace BudgetCast.Expenses.Data
{
    public class ExpensesDbContext : DbContext, IUnitOfWork
    {
        public const string DbSchema = "dbo";

        public DbSet<Expense> Expenses { get; set; }

        public DbSet<Expense> ExpenseItems { get; set; }

        public DbSet<Campaign> Campaigns { get; set; }

        public DbSet<CampaignVm> CampaignsView { get; set; }

        public long Tenant { get; }

        public string UserId { get; }

        public ExpensesDbContext(
            DbContextOptions<ExpensesDbContext> options,
            IIdentityContext identityContext)
            : base(options)
        {
            Tenant = identityContext.TenantId!.Value;
            UserId = identityContext.UserId;

            Expenses = Set<Expense>();
            ExpenseItems = Set<Expense>();
            Campaigns = Set<Campaign>();
            CampaignsView = Set<CampaignVm>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfigurationsFromAssembly(typeof(ExpensesDbContext).Assembly)
                .ApplyConfiguration(new OperationRegistryEntityTypeConfiguration("OperationsRegistry", DbSchema))
                .MarkDateTimeColumnsAsDateTimeInDb();

            modelBuilder
                .AppendGlobalQueryFilter<IMustHaveTenant>(b => b.TenantId == Tenant)
                .AppendGlobalQueryFilter<ISoftDelete>(s => s.DeletedOn == null);
        }

        public async Task<bool> Commit(CancellationToken cancellationToken)
        {
            var now = SystemDt.Current;

            ChangeTracker
                .Entries<IAuditableEntity>()
                .UpdateEntityAuditableValues(now, UserId);

            ChangeTracker
                .Entries<IMustHaveTenant>()
                .UpdateTenantValues(Tenant);

            var result = await base.SaveChangesAsync();
            return result > 0;
        }
    }
}
