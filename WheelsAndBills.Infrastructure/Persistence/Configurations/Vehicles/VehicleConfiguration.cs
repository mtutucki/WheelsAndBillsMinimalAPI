using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Domain.Entities.Vehicles;

namespace Infrastructure.Persistence.Configurations.Vehicles
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Vin)
                .IsRequired()
                .HasMaxLength(17);

            builder.HasIndex(v => v.Vin)
                .IsUnique();

            builder.HasOne(v => v.User)
                .WithMany(u => u.Vehicles)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(v => v.Brand)
                .WithMany()
                .HasForeignKey(v => v.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Model)
                .WithMany()
                .HasForeignKey(v => v.ModelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Type)
                .WithMany()
                .HasForeignKey(v => v.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(v => v.Status)
                .WithMany()
                .HasForeignKey(v => v.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
