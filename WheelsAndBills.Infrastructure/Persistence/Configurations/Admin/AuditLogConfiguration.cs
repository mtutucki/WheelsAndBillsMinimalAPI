using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WheelsAndBills.Domain.Entities.Admin;

namespace WheelsAndBills.Infrastructure.Persistence.Configurations.Admin
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Method)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.Path)
                .HasMaxLength(400)
                .IsRequired();

            builder.Property(x => x.StatusCode)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
