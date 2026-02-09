using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WheelsAndBills.Domain.Entities.Admin;

namespace WheelsAndBills.Infrastructure.Persistence.Configurations.Admin
{
    public class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
    {
        public void Configure(EntityTypeBuilder<ErrorLog> builder)
        {
            builder.ToTable("ErrorLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Source)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Message)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(x => x.Path)
                .HasMaxLength(400);

            builder.Property(x => x.Method)
                .HasMaxLength(10);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(512);

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
