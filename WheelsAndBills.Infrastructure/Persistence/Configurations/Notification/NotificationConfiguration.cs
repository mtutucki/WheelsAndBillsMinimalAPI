using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Domain.Entities.Notification;

namespace Infrastructure.Persistence.Configurations.Notifications
{
    public class NotificationConfiguration
    : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(1000);

            builder.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.Vehicle)
                .WithMany()
                .HasForeignKey(n => n.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
