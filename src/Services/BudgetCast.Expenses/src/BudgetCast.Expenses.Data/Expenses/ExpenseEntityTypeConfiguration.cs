using BudgetCast.Expenses.Domain.Campaigns;
using BudgetCast.Expenses.Domain.Expenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetCast.Expenses.Data.Expenses
{
    internal class ExpenseEntityTypeConfiguration : IEntityTypeConfiguration<Expense>
    {
        private readonly string _schemaName;
        private const string ExpenseIdSeq = nameof(ExpenseIdSeq);

        public ExpenseEntityTypeConfiguration(string schemaName)
        {
            _schemaName = schemaName;
        }
        
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("Expenses", _schemaName);

            builder.HasKey(x => new { x.TenantId, x.Id });

            builder.Property(x => x.Id)
                .UseHiLo(ExpenseIdSeq, _schemaName);

            builder.Ignore(x => x.DomainEvents);

            var expenseItemsNav = builder.Metadata.FindNavigation(nameof(Expense.ExpenseItems));
            expenseItemsNav.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(nameof(Expense.ExpenseItems))
                .WithOne()
                .HasForeignKey("ExpenseTenantId", "ExpenseId");

            var tagsNav = builder.Metadata.FindNavigation(nameof(Expense.Tags));
            tagsNav.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.OwnsMany<Tag>(nameof(Expense.Tags), x =>
            {
                x.ToTable("Tags", _schemaName);
                x.WithOwner().HasForeignKey("ExpenseTenantId", "ExpenseId");
                x.Property<long>("Id");
                x.HasKey("Id");

                x.Property(x => x.Name)
                    .HasMaxLength(100);
            });

            builder.Property(x => x.AddedAt)
                .HasAnnotation(nameof(Expense.AddedAt), "Expense add data");

            builder.Property(x => x.Description)
                .HasMaxLength(150)
                .HasAnnotation(nameof(Expense.Description), "Expense description");

            builder.Property(x => x.TotalPrice)
                .HasAnnotation(nameof(Expense.TotalPrice), "Expense total");

            builder.Property<long>("_campaignId")
                .HasColumnName("CampaignId")
                .IsRequired();

            builder.Property<long>("_campaignTenantId")
                .HasColumnName("CampaignTenantId")
                .IsRequired();

            builder.HasOne<Campaign>()
                .WithMany()
                .HasForeignKey("_campaignTenantId", "_campaignId");
        }
    }
}
