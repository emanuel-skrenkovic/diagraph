using Diagraph.Modules.GlucoseData.Imports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Modules.GlucoseData.Database;

public class ImportConfiguration : IEntityTypeConfiguration<Import>
{
    public void Configure(EntityTypeBuilder<Import> builder)
    {
        builder.ToTable("import");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnName("id");
        
        builder.Property(i => i.UserId).HasColumnName("user_id");

        builder
            .Property(i => i.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamptz");
        builder
            .Property(i => i.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("timestamptz");

        builder.Property(i => i.Hash).HasColumnName("hash");
    }
}