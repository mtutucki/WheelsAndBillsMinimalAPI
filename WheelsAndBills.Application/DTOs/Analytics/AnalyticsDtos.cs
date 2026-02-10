namespace WheelsAndBills.Application.DTOs.Analytics
{
    public record MileagePointDto(
        DateTime Date,
        int Mileage
    );

    public record MileageListItemDto(
        DateTime Date,
        int Mileage
    );

    public record EventStatsPointDto(
        DateTime Date,
        int Count,
        decimal Cost
    );

    public record EventListItemDto(
        Guid Id,
        DateTime EventDate,
        string EventType,
        int Mileage,
        decimal Cost,
        string? Description
    );

    public record FuelStatsPointDto(
        DateTime Date,
        int Mileage,
        int Distance,
        decimal Liters,
        decimal TotalCost,
        decimal PricePerLiter,
        decimal ConsumptionPer100,
        decimal CostPer100
    );

    public record FuelListItemDto(
        DateTime Date,
        int Mileage,
        decimal Liters,
        decimal TotalCost,
        decimal PricePerLiter
    );
}
