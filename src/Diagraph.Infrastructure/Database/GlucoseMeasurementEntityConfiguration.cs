using Diagraph.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Infrastructure.Database;

public class GlucoseMeasurementEntityConfiguration : IEntityTypeConfiguration<GlucoseMeasurement>
{
    public void Configure(EntityTypeBuilder<GlucoseMeasurement> builder)
    {
        builder.ToTable("glucose_measurement");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id");
        
        builder.Property(m => m.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(m => m.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.Property(m => m.Level).HasColumnName("level");
        builder.Property(m => m.TakenAt).HasColumnName("taken_at");
        builder.Property(m => m.Unit).HasColumnName("unit");

        builder.Property(m => m.ImportId).HasColumnName("import_id");

        builder
            .HasOne<Import>()
            .WithMany(i => i.Measurements)
            .HasForeignKey(m => m.ImportId);
    }
}