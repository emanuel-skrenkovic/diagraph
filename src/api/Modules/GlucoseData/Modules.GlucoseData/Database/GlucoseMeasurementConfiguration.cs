using Diagraph.Modules.GlucoseData.Imports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Modules.GlucoseData.Database;

public class GlucoseMeasurementConfiguration : IEntityTypeConfiguration<GlucoseMeasurement>
{
    public void Configure(EntityTypeBuilder<GlucoseMeasurement> builder)
    {
        RelationalEntityTypeBuilderExtensions.ToTable((EntityTypeBuilder)builder, "glucose_measurement");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id");
        
        builder.Property(m => m.UserId).HasColumnName("user_id");
        
        builder
            .Property(m => m.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamptz");
        
        builder
            .Property(m => m.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("timestamptz");
        builder.Property(m => m.Level).HasColumnName("level");
        builder
            .Property(m => m.TakenAt)
            .HasColumnName("taken_at")
            .HasColumnType("timestamptz");
        builder.Property(m => m.Unit).HasColumnName("unit");

        builder.Property(m => m.ImportId).HasColumnName("import_id");

        builder
            .HasOne<Import>()
            .WithMany(i => i.Measurements)
            .HasForeignKey(m => m.ImportId);
    }
}