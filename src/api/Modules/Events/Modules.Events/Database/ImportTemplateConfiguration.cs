using Diagraph.Modules.Events.DataImports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Modules.Events.Database;

public class ImportTemplateConfiguration : IEntityTypeConfiguration<ImportTemplate>
{
    public void Configure(EntityTypeBuilder<ImportTemplate> builder)
    {
        builder.ToTable("import_template");

        builder.HasKey(t => t.Id);
        builder
            .Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder
            .Property(t => t.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamp");
        
        builder
            .Property(t => t.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("timestamp");

        builder.Property(t => t.Name).HasColumnName("name");
        builder
            .Property(t => t.Data)
            .HasColumnName("data")
            .HasColumnType("jsonb");
    }
}