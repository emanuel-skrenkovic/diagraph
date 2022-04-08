using Diagraph.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Infrastructure.Database.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");

        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Id).HasColumnName("id");
        builder.Property(u => u.Email).HasColumnName("email");
        builder.Property(u => u.PasswordHash).HasColumnName("password_hash");
        builder.Property(u => u.Locked).HasColumnName("locked");
        builder.Property(u => u.UnsuccessfulLoginAttempts).HasColumnName("unsuccessful_login_attempts");
        builder.Property(u => u.EmailConfirmed).HasColumnName("email_confirmed");
        builder.Property(u => u.SecurityStamp).HasColumnName("security_stamp");

        builder.HasMany<Event>().WithOne().HasForeignKey(e => e.UserId);
        builder.HasMany<Tag>().WithOne().HasForeignKey(e => e.UserId);
        builder.HasMany<GlucoseMeasurement>().WithOne().HasForeignKey(e => e.UserId);
        builder.HasMany<Import>().WithOne().HasForeignKey(e => e.UserId);
    }
}