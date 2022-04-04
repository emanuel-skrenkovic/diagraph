using Diagraph.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Infrastructure.Database.EntityConfigurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profile");

        builder.HasKey(p => p.Id);
        builder
            .Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        
         builder
             .Property(p => p.CreatedAtUtc)
             .HasColumnName("created_at_utc")
             .HasColumnType("timestamptz");
         
         builder
             .Property(p => p.UpdatedAtUtc)
             .HasColumnName("updated_at_utc")
             .HasColumnType("timestamptz");

         builder.Property(p => p.UserId).HasColumnName("user_id");

         builder
             .Property(p => p.Data)
             .HasColumnName("data")
             .HasColumnType("jsonb");
    }
}