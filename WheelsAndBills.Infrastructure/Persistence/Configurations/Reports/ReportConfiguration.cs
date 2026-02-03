using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Domain.Entities.Report;

namespace Infrastructure.Persistence.Configurations.Reports
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasOne(r => r.User)
                .WithMany(u => u.Reports)
                .HasForeignKey(r => r.UserId);

            builder.HasOne(r => r.Definition)
                .WithMany()
                .HasForeignKey(r => r.ReportDefinitionId);
        }
    }

}
