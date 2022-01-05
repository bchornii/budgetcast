using BudgetCast.Expenses.Domain.Campaigns;
using BudgetCast.Expenses.Domain.Expenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetCast.Expenses.Data.Expenses
{
    internal class ExpenseEntityTypeConfiguration : IEntityTypeConfiguration<Expense>
    {
        private const string ExpenseIdSeq = nameof(ExpenseIdSeq);

        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("Expenses", ExpensesDbContext.DbSchema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .UseHiLo(ExpenseIdSeq, ExpensesDbContext.DbSchema);

            builder.Ignore(x => x.DomainEvents);

            var expenseItemsNav = builder.Metadata.FindNavigation(nameof(Expense.ExpenseItems));
            expenseItemsNav.SetPropertyAccessMode(PropertyAccessMode.Field);

            var tagsNav = builder.Metadata.FindNavigation(nameof(Expense.Tags));
            tagsNav.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.OwnsMany<Tag>(nameof(Expense.Tags), x =>
            {
                x.ToTable("Tags", ExpensesDbContext.DbSchema);
                x.WithOwner().HasForeignKey("ExpenseId");
                x.Property<int>("Id");
                x.HasKey("Id");

                x.Property(x => x.Name)
                    .HasMaxLength(100);

                x.Property(x => x.ExpenseId)
                    .IsRequired();
                    
            });

            builder.Property(x => x.AddedAt)
                .HasAnnotation(nameof(Expense.AddedAt), "Expense add data");

            builder.Property(x => x.Description)
                .HasMaxLength(150)
                .HasAnnotation(nameof(Expense.Description), "Expense description");

            builder.HasOne<Campaign>()
                .WithMany()
                .HasForeignKey("_campaignId");
        }
    }
}
