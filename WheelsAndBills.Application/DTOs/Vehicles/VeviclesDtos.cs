namespace WheelsAndBills.Application.DTOs.Vehicles
{
    public record CreateMyVehicleNoteDTO(
        Guid VehicleId,
        string Content
    );

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





    public record CreateVehicleDTO(
        Guid UserId,
        string Vin,
        int Year,
        Guid BrandId,
        Guid ModelId,
        Guid TypeId,
        Guid StatusId
    );

    public record UpdateVehicleDTO(
        string Vin,
        int Year,
        Guid BrandId,
        Guid ModelId,
        Guid TypeId,
        Guid StatusId
    );

    public record GetVehicleDTO(
        Guid Id,
        Guid UserId,
        string Vin,
        int Year,
        Guid BrandId,
        Guid ModelId,
        Guid TypeId,
        Guid StatusId
    );


    public record CreateVehicleBrandDTO(string Name);

    public record UpdateVehicleBrandDTO(string Name);

    public record GetVehicleBrandDTO(Guid Id, string Name);




    public record CreateVehicleMileageDTO(
        Guid VehicleId,
        int Mileage,
        DateTime Date
    );

    public record UpdateVehicleMileageDTO(
        int Mileage,
        DateTime Date
    );

    public record GetVehicleMileageDTO(
        Guid Id,
        Guid VehicleId,
        int Mileage,
        DateTime Date
    );


    public record CreateVehicleModelDTO(
        Guid BrandId,
        string Name
    );

    public record UpdateVehicleModelDTO(
        Guid BrandId,
        string Name
    );

    public record GetVehicleModelDTO(
        Guid Id,
        Guid BrandId,
        string Name
    );

    public record CreateVehicleNoteDTO(
        Guid VehicleId,
        Guid UserId,
        string Content
    );

    public record UpdateVehicleNoteDTO(
        string Content
    );

    public record GetVehicleNoteDTO(
        Guid Id,
        Guid VehicleId,
        Guid UserId,
        string Content,
        DateTime CreatedAt
    );


    public record CreateVehicleStatusDTO(string Name);

    public record UpdateVehicleStatusDTO(string Name);

    public record GetVehicleStatusDTO(Guid Id, string Name);

    public record CreateVehicleTypeDTO(string Name);

    public record UpdateVehicleTypeDTO(string Name);

    public record GetVehicleTypeDTO(Guid Id, string Name);

}


;
