using Diagraph.Modules.Identity.ExternalIntegrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Modules.Identity.Database;

public class ExternalConfiguration : IEntityTypeConfiguration<External>
{
    public void Configure(EntityTypeBuilder<External> builder)
    {
        builder.ToTable("external");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("id");
        
        builder
            .Property(et => et.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamptz");
        
        builder
            .Property(et => et.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("timestamptz");
        
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.Provider).HasColumnName("provider");
        builder.Property(e => e.Data).HasColumnName("data").HasColumnType("jsonb");

        builder.HasOne<User>().WithMany().HasForeignKey(e => e.UserId);
    }
}