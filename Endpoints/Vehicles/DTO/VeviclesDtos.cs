namespace WheelsAndBillsAPI.Endpoints.Vehicles.DTO
{
    public record CreateVehicleRequestDTO(
            string Vin,
            int Year,
            Guid BrandId,
            Guid ModelId,
            Guid TypeId,
            Guid StatusId
     );

    public record GetVehiclesByUserDTO(
        Guid Id,
        string Vin,
        int Year,

        LookupDTO Brand,
        LookupDTO Model,
        LookupDTO Type,
        LookupDTO Status
    );

    public record LookupDTO(Guid Id, string Name);


    public record VehicleMileageDTO(
        Guid Id,
        int Mileage,
        DateTime Date
    );

    public record VehicleEventDTO(
        Guid Id,
        LookupDTO EventType,
        DateTime EventDate,
        int Mileage,
        string? Description
    );

    public record VehicleNoteDTO(
        Guid Id,
        string Content,
        DateTime CreatedAt,
        Guid UserId
    );

    public record VehicleDetailsDTO(
        Guid Id,
        string Vin,
        int Year,

        LookupDTO Brand,
        LookupDTO Model,
        LookupDTO Type,
        LookupDTO Status,

        IReadOnlyList<VehicleMileageDTO> Mileage,
        IReadOnlyList<VehicleEventDTO> Events,
        IReadOnlyList<VehicleNoteDTO> Notes
    );
}

;