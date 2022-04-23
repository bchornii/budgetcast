using BudgetCast.Expenses.Domain.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetCast.Expenses.Data.Campaigns
{
    internal class CampaignEntityTypeConfiguration : IEntityTypeConfiguration<Campaign>
    {
        private readonly string _schemaName;
        private const string CampaignIdSeq = nameof(CampaignIdSeq);

        public CampaignEntityTypeConfiguration(string schemaName)
        {
            _schemaName = schemaName;
        }
        
        public void Configure(EntityTypeBuilder<Campaign> builder)
        {
            builder.ToTable("Campaigns", _schemaName);

            builder.HasKey(x => new { x.TenantId, x.Id });

            builder.Property(x => x.Id)
                .UseHiLo(CampaignIdSeq, _schemaName);

            builder.Ignore(x => x.DomainEvents);

            builder.Property(x => x.Name)
                .HasColumnType("NVARCHAR(50)")
                .HasAnnotation(nameof(Campaign.Name), "Campaign Name");

            builder.Property(x => x.StartsAt)
                .HasAnnotation(nameof(Campaign.StartsAt), "Campaign Start Date");

            builder.Property(x => x.CompletesAt)
                .HasAnnotation(nameof(Campaign.CompletesAt), "Campaign Completion Date");
        }
    }
}
