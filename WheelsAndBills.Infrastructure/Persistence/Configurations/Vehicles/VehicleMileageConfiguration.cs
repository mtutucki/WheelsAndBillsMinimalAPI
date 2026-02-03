using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Domain.Entities.Vehicles;

namespace Infrastructure.Persistence.Configurations.Vehicles
{
    public class VehicleMileageConfiguration : IEntityTypeConfiguration<VehicleMileage>
    {
        public void Configure(EntityTypeBuilder<VehicleMileage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Mileage)
                .IsRequired();

            builder.HasOne(x => x.Vehicle)
                .WithMany(v => v.Mileages)
                .HasForeignKey(x => x.VehicleId);
        }
    }

}
