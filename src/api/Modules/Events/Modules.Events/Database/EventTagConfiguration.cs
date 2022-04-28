using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Modules.Events.Database;

public class EventTagConfiguration : IEntityTypeConfiguration<EventTag>
{
    public void Configure(EntityTypeBuilder<EventTag> builder)
    {
        builder.ToTable("event_tag");

        builder.HasKey(et => new { et.EventId, et.Name });
        
        builder
            .Property(et => et.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamptz");
        
        builder
            .Property(et => et.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("timestamptz");
        
        builder.Property(et => et.EventId).HasColumnName("event_id");
        builder.Property(et => et.Name).HasColumnName("name");

        builder
            .HasOne<Event>()
            .WithMany(e => e.Tags)
            .HasForeignKey(et => et.EventId);
    }
}