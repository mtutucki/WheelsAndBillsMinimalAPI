using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Domain.Entities.Events;

namespace Infrastructure.Persistence.Configurations.Events
{
    public class EventPartConfiguration : IEntityTypeConfiguration<EventPart>
    {
        public void Configure(EntityTypeBuilder<EventPart> builder)
        {
            builder.HasKey(ep => new { ep.RepairEventId, ep.PartId });

            builder.Property(ep => ep.Price)
                .HasPrecision(10, 2);

            builder.HasOne(ep => ep.RepairEvent)
                .WithMany(r => r.Parts)
                .HasForeignKey(ep => ep.RepairEventId);

            builder.HasOne(ep => ep.Part)
                .WithMany()
                .HasForeignKey(ep => ep.PartId);
        }
    }

}
