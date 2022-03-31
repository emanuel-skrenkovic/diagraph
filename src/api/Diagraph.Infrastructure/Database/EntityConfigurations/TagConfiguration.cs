using Diagraph.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Infrastructure.Database.EntityConfigurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tag");

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

        builder.Property(t => t.Name).HasColumnName("name");
    }
}