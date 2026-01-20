using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Domain.Entities.Auth;
using WheelsAndBillsAPI.Domain.Entities.Report;
using WheelsAndBillsAPI.Domain.Entities.Vehicles;
using WheelsAndBillsAPI.Domain.Entities.Admin;
using WheelsAndBillsAPI.Domain.Entities.Events;
using WheelsAndBillsAPI.Domain.Entities.Cost;
using WheelsAndBillsAPI.Domain.Entities.Notification;

namespace WheelsAndBillsAPI.Persistence
{
    public class AppDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // Vehicles
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<VehicleBrand> VehicleBrands => Set<VehicleBrand>();
        public DbSet<VehicleModel> VehicleModels => Set<VehicleModel>();
        public DbSet<VehicleType> VehicleTypes => Set<VehicleType>();
        public DbSet<VehicleStatus> VehicleStatuses => Set<VehicleStatus>();
        public DbSet<VehicleMileage> VehicleMileage => Set<VehicleMileage>();
        public DbSet<VehicleNote> VehicleNotes => Set<VehicleNote>();

        // Events
        public DbSet<VehicleEvent> VehicleEvents => Set<VehicleEvent>();
        public DbSet<EventType> EventTypes => Set<EventType>();
        public DbSet<ServiceEvent> ServiceEvents => Set<ServiceEvent>();
        public DbSet<RepairEvent> RepairEvents => Set<RepairEvent>();
        public DbSet<FuelingEvent> FuelingEvents => Set<FuelingEvent>();
        public DbSet<EventPart> EventParts => Set<EventPart>();
        public DbSet<Part> Parts => Set<Part>();
        public DbSet<Workshop> Workshops => Set<Workshop>();

        // Costs
        public DbSet<Cost> Costs => Set<Cost>();
        public DbSet<CostType> CostTypes => Set<CostType>();
        public DbSet<RecurringCost> RecurringCosts => Set<RecurringCost>();

        // Notifications
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<NotificationType> NotificationTypes => Set<NotificationType>();

        // Reports
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<ReportDefinition> ReportDefinitions => Set<ReportDefinition>();
        public DbSet<ReportParameter> ReportParameters => Set<ReportParameter>();
        public DbSet<GeneratedReport> GeneratedReports => Set<GeneratedReport>();

        // CMS / Admin
        public DbSet<ContentPage> ContentPage => Set<ContentPage>();
        public DbSet<ContentBlock> ContentBlocks => Set<ContentBlock>();
        public DbSet<Dictionary> Dictionaries => Set<Dictionary>();
        public DbSet<DictionaryItem> DictionaryItems => Set<DictionaryItem>();
        public DbSet<FileResource> FileResources => Set<FileResource>();
        public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
