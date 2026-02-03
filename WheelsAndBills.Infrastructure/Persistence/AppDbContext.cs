using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Domain.Entities.Auth;
using WheelsAndBills.Domain.Entities.Report;
using WheelsAndBills.Domain.Entities.Vehicles;
using WheelsAndBills.Domain.Entities.Admin;
using WheelsAndBills.Domain.Entities.Events;
using WheelsAndBills.Domain.Entities.Cost;
using WheelsAndBills.Domain.Entities.Notification;

namespace WheelsAndBills.Infrastructure.Persistence
{
    public class AppDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IAppDbContext
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
