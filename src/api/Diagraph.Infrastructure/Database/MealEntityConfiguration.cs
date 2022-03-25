using Diagraph.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Infrastructure.Database;

public class MealEntityConfiguration : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> builder)
    {
        builder.ToTable("meal");

        builder.HasKey(m => m.Id);
        builder
            .Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder
            .Property(m => m.OccurredAtUtc)
            .HasColumnName("occurred_at_utc")
            .HasColumnType("timestamptz");
        builder
            .Property(m => m.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamptz");
        builder
            .Property(m => m.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("timestamptz");
        builder.Property(m => m.Text).HasColumnName("text");
        builder.Property(m => m.Type).HasColumnName("type");
    }
}