using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Domain.Entities.Events;

namespace Infrastructure.Persistence.Configurations.Events
{
    public class RepairEventConfiguration : IEntityTypeConfiguration<RepairEvent>
    {
        public void Configure(EntityTypeBuilder<RepairEvent> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.LaborCost)
                .HasPrecision(10, 2);

            builder.HasOne(r => r.VehicleEvent)
                .WithOne()
                .HasForeignKey<RepairEvent>(r => r.VehicleEventId);
        }
    }

}
