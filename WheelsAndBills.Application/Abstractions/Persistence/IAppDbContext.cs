using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WheelsAndBills.Domain.Entities.Admin;
using WheelsAndBills.Domain.Entities.Auth;
using WheelsAndBills.Domain.Entities.Cost;
using WheelsAndBills.Domain.Entities.Events;
using WheelsAndBills.Domain.Entities.Notification;
using WheelsAndBills.Domain.Entities.Report;
using WheelsAndBills.Domain.Entities.Vehicles;
using static WheelsAndBills.Application.DTOs.Reports.ReportDTOs;

namespace WheelsAndBills.Application.Abstractions.Persistence
{
    public interface IAppDbContext
    {
        DatabaseFacade Database { get; }

        DbSet<ApplicationUser> Users { get; }

        DbSet<Vehicle> Vehicles { get; }
        DbSet<VehicleBrand> VehicleBrands { get; }
        DbSet<VehicleModel> VehicleModels { get; }
        DbSet<VehicleType> VehicleTypes { get; }
        DbSet<VehicleStatus> VehicleStatuses { get; }
        DbSet<VehicleMileage> VehicleMileage { get; }
        DbSet<VehicleNote> VehicleNotes { get; }

        DbSet<VehicleEvent> VehicleEvents { get; }
        DbSet<EventType> EventTypes { get; }
        DbSet<ServiceEvent> ServiceEvents { get; }
        DbSet<RepairEvent> RepairEvents { get; }
        DbSet<FuelingEvent> FuelingEvents { get; }
        DbSet<EventPart> EventParts { get; }
        DbSet<Part> Parts { get; }
        DbSet<Workshop> Workshops { get; }

        DbSet<Cost> Costs { get; }
        DbSet<CostType> CostTypes { get; }
        DbSet<RecurringCost> RecurringCosts { get; }

        DbSet<Notification> Notifications { get; }
        DbSet<NotificationType> NotificationTypes { get; }
        DbSet<NotificationPreference> NotificationPreferences { get; }

        DbSet<Report> Reports { get; }
        DbSet<ReportDefinition> ReportDefinitions { get; }
        DbSet<ReportParameter> ReportParameters { get; }
        DbSet<GeneratedReport> GeneratedReports { get; }

        DbSet<ContentPage> ContentPage { get; }
        DbSet<ContentBlock> ContentBlocks { get; }
        DbSet<Dictionary> Dictionaries { get; }
        DbSet<DictionaryItem> DictionaryItems { get; }
        DbSet<FileResource> FileResources { get; }
        DbSet<SystemSetting> SystemSettings { get; }
        DbSet<AuditLog> AuditLogs { get; }
        DbSet<ErrorLog> ErrorLogs { get; }

        public DbSet<MonthlyCostRow> MonthlyCostRows { get; }
        public DbSet<CostsByEventTypeRow> CostsByEventTypeRows { get; }
        public DbSet<RepairHistoryRow> RepairHistoryRows { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
