using BudgetCast.Common.Application.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetCast.Common.Data.EventLog;

public class IntegrationEventLogEntryTypeConfiguration : IEntityTypeConfiguration<IntegrationEventLogEntry>
{
    private readonly string _tableName;
    private readonly string _schemaName;

    public IntegrationEventLogEntryTypeConfiguration(string tableName, string schemaName)
    {
        _tableName = tableName;
        _schemaName = schemaName;
    }
    
    public void Configure(EntityTypeBuilder<IntegrationEventLogEntry> builder)
    {
        builder.ToTable(_tableName, _schemaName);

        builder.HasKey(e => e.EventId);

        builder.Property(e => e.EventId)
            .IsRequired();

        builder.Property(e => e.Content)
            .IsRequired();

        builder.Property(e => e.CreationTime)
            .IsRequired();

        builder.Property(e => e.State)
            .IsRequired();

        builder.Property(e => e.TimesSent)
            .IsRequired();

        builder.Property(e => e.EventTypeName)
            .IsRequired();
    }
}