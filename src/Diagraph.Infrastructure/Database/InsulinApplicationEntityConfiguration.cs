using Diagraph.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Infrastructure.Database;

public class InsulinApplicationEntityConfiguration : IEntityTypeConfiguration<InsulinApplication>
{
    public void Configure(EntityTypeBuilder<InsulinApplication> builder)
    {
        builder.ToTable("insulin_application");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnName("id");
        builder.Property(i => i.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(i => i.UpdatedAtUtc).HasColumnName("updated_at_utc");
        builder.Property(i => i.Units).HasColumnName("units");
        builder.Property(i => i.MealId).HasColumnName("meal_id");

        builder
            .HasOne<Meal>()
            .WithMany(m => m.InsulinApplications)
            .HasForeignKey(i => i.MealId);
    }
}