namespace WheelsAndBills.Application.DTOs.Dashboard
{
    public record DashboardDto(
        UserSummaryDto User,
        List<VehicleSummaryDto> Vehicles,
        DashboardStatsDto Stats,
        Dictionary<Guid, List<VehicleEventDto>> RecentEventsByVehicle,
        Dictionary<Guid, decimal> PerVehicleTotalCost,
        Dictionary<Guid, DateTime?> PerVehicleLastFueling,
        List<ServiceReminderDto> ServiceReminders
    );

    public record UserSummaryDto(Guid Id, string Name, string LastName, string Email);

    public record VehicleSummaryDto(
        Guid Id,
        string Brand,
        string Model,
        int Year,
        string Status,
        int? CurrentMileage
    );

    public record DashboardStatsDto(
        int VehiclesCount,
        decimal TotalSpentAllVehicles,
        decimal RecurringMonthlyTotal,
        DateTime? LastFuelingDate,
        int NewEventsCount
    );

    public record VehicleEventDto(
        Guid Id,
        string EventType,
        DateTime Date,
        int? Mileage,
        decimal? TotalCost
    );

    public record ServiceReminderDto(
        Guid VehicleId,
        string VehicleName,
        DateTime? LastServiceDate,
        int? LastServiceMileage,
        DateTime? NextServiceDate,
        int? NextServiceMileage,
        bool IsOverdueDate,
        bool IsOverdueMileage
    );

    public record CostCompareVehicleDto(
        Guid VehicleId,
        string Label,
        decimal Total,
        decimal Fuel,
        decimal Repairs,
        decimal Parts,
        decimal AvgTotal,
        decimal AvgFuel,
        decimal AvgRepairs,
        decimal AvgParts
    );

    public record CostCompareAveragesDto(
        decimal Total,
        decimal Fuel,
        decimal Repairs,
        decimal Parts
    );

    public record CostCompareResultDto(
        string Range,
        List<CostCompareVehicleDto> Vehicles,
        CostCompareAveragesDto Average
    );
}
