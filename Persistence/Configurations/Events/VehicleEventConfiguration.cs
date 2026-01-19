using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Domain.Entities.Events;

namespace Infrastructure.Persistence.Configurations.Events
{
    public class VehicleEventConfiguration : IEntityTypeConfiguration<VehicleEvent>
    {
        public void Configure(EntityTypeBuilder<VehicleEvent> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.EventDate)
                .IsRequired();

            builder.HasOne(e => e.Vehicle)
                .WithMany(v => v.Events)
                .HasForeignKey(e => e.VehicleId);

            builder.HasOne(e => e.EventType)
                .WithMany()
                .HasForeignKey(e => e.EventTypeId);
        }
    }

}
