using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Domain.Entities.Admin;

namespace Infrastructure.Persistence.Configurations.Admin
{
    public class ContentPageConfiguration : IEntityTypeConfiguration<ContentPage>
    {
        public void Configure(EntityTypeBuilder<ContentPage> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(x => x.Slug)
                .IsUnique();
        }
    }

}
