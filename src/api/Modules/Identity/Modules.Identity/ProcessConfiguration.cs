using Diagraph.Infrastructure.ProcessManager.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Modules.Identity;

public class ProcessConfiguration : IEntityTypeConfiguration<Process>
{
    public void Configure(EntityTypeBuilder<Process> builder)
    {
        builder.ToTable("process");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");

        builder
            .Property(p => p.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .HasColumnType("timestamptz");
        
        builder
            .Property(p => p.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .HasColumnType("timestamptz");
        
        builder.Property(p => p.ProcessId).HasColumnName("process_id");
        builder.Property(p => p.Initiated).HasColumnName("initiated");
        builder.Property(p => p.Finished).HasColumnName("finished");
        
        builder
            .Property(p => p.Data)
            .HasColumnName("data")
            .HasColumnType("jsonb");
    }
}