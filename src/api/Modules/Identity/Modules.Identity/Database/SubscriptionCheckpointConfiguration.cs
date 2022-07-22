using Diagraph.Infrastructure.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Modules.Identity.Database;

public class SubscriptionCheckpointConfiguration : IEntityTypeConfiguration<SubscriptionCheckpoint>
{
    public void Configure(EntityTypeBuilder<SubscriptionCheckpoint> builder)
    {
        builder.ToTable("subscription_checkpoint");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.SubscriptionId).HasColumnName("subscription_id");
        builder.Property(c => c.Position).HasColumnName("position");
    }
}