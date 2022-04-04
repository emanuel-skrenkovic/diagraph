using Diagraph.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Infrastructure.Database.EntityConfigurations;

public class EventTagConfiguration : IEntityTypeConfiguration<EventTag>
{
    public void Configure(EntityTypeBuilder<EventTag> builder)
    {
        builder.ToTable("event_tag");

        builder.HasKey(et => new { et.EventId, et.TagId });
        
        builder
            .Property(et => et.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamptz");
        
        builder
            .Property(et => et.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("timestamptz");
        
        builder.Property(et => et.EventId).HasColumnName("event_id");
        builder.Property(et => et.TagId).HasColumnName("tag_id");

        builder
            .HasOne<Event>()
            .WithMany(e => e.Tags)
            .HasForeignKey(et => et.EventId);

        builder
            .HasOne<Tag>()
            .WithMany()
            .HasForeignKey(et => et.TagId);
    }
}