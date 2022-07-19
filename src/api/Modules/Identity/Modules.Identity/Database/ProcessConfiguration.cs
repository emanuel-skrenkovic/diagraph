using Diagraph.Infrastructure.ProcessManager.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diagraph.Modules.Identity.Database;

public class ProcessConfiguration : IEntityTypeConfiguration<Process>
{
    public void Configure(EntityTypeBuilder<Process> builder)
    {
        builder.ToTable("process");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id).HasColumnName("id");
        builder.Property(r => r.ProcessId).HasColumnName("process_id");
        builder.Property(r => r.Data).HasColumnName("data").HasColumnType("jsonb");
    }
}