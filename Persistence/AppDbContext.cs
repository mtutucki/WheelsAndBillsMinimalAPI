using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Domain.Entities.Auth;
using WheelsAndBillsAPI.Domain.Entities.Report;
using WheelsAndBillsAPI.Domain.Entities.Vehicles;
using WheelsAndBillsAPI.Domain.Entities.Admin;

namespace WheelsAndBillsAPI.Persistence
{
    public class AppDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<Report> Reports => Set<Report>();

        public DbSet<ContentPage> ContentPage => Set<ContentPage>();
        public DbSet<ContentBlock> ContentBlocks => Set<ContentBlock>();


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
