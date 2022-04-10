using BudgetCast.Common.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace BudgetCast.Common.Data;

public sealed class OperationRegistryEntityTypeConfiguration : IEntityTypeConfiguration<OperationRegistryEntity>
{
    private readonly string _tableName;
    private readonly string _schemaName;

    public OperationRegistryEntityTypeConfiguration(string tableName, string schemaName)
    {
        _tableName = tableName;
        _schemaName = schemaName;
    }

    public void Configure(EntityTypeBuilder<OperationRegistryEntity> builder)
    {
        builder.ToTable(_tableName, _schemaName);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired();

        builder.Property(e => e.Operations)
            .HasColumnType("NVARCHAR(MAX)")
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<ICollection<OperationPart>>(v));

        builder.Property(e => e.IdempodentOperationName)
            .IsRequired();

        builder.Property(e => e.StartedOn)
            .IsRequired();

        builder.Property(e => e.CorrelationId)
            .IsRequired();

        builder.Property(e => e.OperationResult);
    }
}