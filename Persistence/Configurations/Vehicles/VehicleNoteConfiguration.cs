using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Domain.Entities.Vehicles;

namespace Infrastructure.Persistence.Configurations.Vehicles
{
    public class VehicleNoteConfiguration
    : IEntityTypeConfiguration<VehicleNote>
    {
        public void Configure(EntityTypeBuilder<VehicleNote> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Content)
                .IsRequired()
                .HasMaxLength(2000);

            builder.HasOne(n => n.Vehicle)
                .WithMany()
                .HasForeignKey(n => n.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
