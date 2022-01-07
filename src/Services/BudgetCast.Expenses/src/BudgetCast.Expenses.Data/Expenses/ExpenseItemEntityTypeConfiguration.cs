using BudgetCast.Expenses.Domain.Expenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetCast.Expenses.Data.Expenses
{
    internal class ExpenseItemEntityTypeConfiguration : IEntityTypeConfiguration<ExpenseItem>
    {
        private const string ExpenseItemIdSeq = nameof(ExpenseItemIdSeq);

        public void Configure(EntityTypeBuilder<ExpenseItem> builder)
        {
            builder.ToTable("ExpenseItems", ExpensesDbContext.DbSchema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .UseHiLo(ExpenseItemIdSeq, ExpensesDbContext.DbSchema);

            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.Title)
                .HasMaxLength(150)
                .HasAnnotation(nameof(ExpenseItem.Title), "Expense item Title");

            builder.Property(x => x.Note)
                .HasMaxLength(300)
                .HasAnnotation(nameof(ExpenseItem.Note), "Expense item Note");

            builder.Property(x => x.Price)
                .HasPrecision(18, 5)
                .HasAnnotation(nameof(ExpenseItem.Price), "Expense item Price");

            builder.Property(x => x.Quantity)
                .HasAnnotation(nameof(ExpenseItem.Quantity), "Expense item Quatity");
        }
    }
}
