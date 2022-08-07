using Diagraph.Modules.Events.DataExports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Modules.Events.Database;

public class ExportTemplateConfiguration : IEntityTypeConfiguration<ExportTemplate>
{
    public void Configure(EntityTypeBuilder<ExportTemplate> builder)
    {
        builder.ToTable("export_template");

        builder.HasKey(t => t.Id);
        builder
            .Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
        builder
            .Property(t => t.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamptz");
        
        builder
            .Property(t => t.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("timestamptz");

        builder.Property(t => t.UserId).HasColumnName("user_id");
        builder.Property(t => t.Name).HasColumnName("name");

        builder
            .Property(t => t.Data)
            .HasColumnName("data")
            .HasColumnType("jsonb");
    }
}