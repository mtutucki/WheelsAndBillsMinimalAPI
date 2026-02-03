using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Domain.Entities.Cost;

namespace Infrastructure.Persistence.Configurations.Costs
{
    public class CostConfiguration : IEntityTypeConfiguration<Cost>
    {
        public void Configure(EntityTypeBuilder<Cost> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Amount)
                .HasPrecision(10, 2);

            builder.HasOne(c => c.VehicleEvent)
                .WithMany()
                .HasForeignKey(c => c.VehicleEventId);

            builder.HasOne(c => c.CostType)
                .WithMany()
                .HasForeignKey(c => c.CostTypeId);
        }
    }

}
