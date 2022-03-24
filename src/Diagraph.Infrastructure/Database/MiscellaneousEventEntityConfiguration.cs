using Diagraph.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Infrastructure.Database;

public class MiscellaneousEventEntityConfiguration : IEntityTypeConfiguration<MiscellanousEvent>
{
    public void Configure(EntityTypeBuilder<MiscellanousEvent> builder)
    {
        builder.ToTable("miscellaneous_event");

        builder.HasKey(e => e.Id);
        builder
            .Property(e => e.Id)
            .HasColumnName("id");
        
        builder.Property(e => e.CreatedAtUtc).HasColumnName("created_at_utc");
        builder.Property(e => e.UpdatedAtUtc).HasColumnName("updated_at_utc");

        builder.Property(i => i.Note).HasColumnName("note");
    }
}