using BudgetCast.Common.Authentication;
using BudgetCast.Common.Domain;
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

        public ExpensesDbContext(DbContextOptions<ExpensesDbContext> options, IIdentityContext identityContext)
            : base(options)
        {
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
        }

        public async Task<bool> Commit()
        {
            ChangeTracker.DetectChanges();
            foreach (var entity in ChangeTracker.Entries())
            {
                if(entity.Entity is AggregateRoot aggregate)
                {
                    if (entity.State == EntityState.Added)
                    {
                        aggregate.SetCreationDetails(_identityContext.UserId, SystemDt.Current);
                    }

                    if(entity.State == EntityState.Modified)
                    {
                        // TODO: this won't work for included entities, because we're checking only aggregate root here
                        // TODO: need to come up with a fix
                        aggregate.SetUpdateDetails(_identityContext.UserId, SystemDt.Current);
                    }
                }
            }
            var result = await base.SaveChangesAsync();
            return result > 0;
        }
    }
}
