namespace WheelsAndBillsAPI.Endpoints.Events
{
    public class EventsDTO
    {
        public record CreateEventPartDTO(
            Guid RepairEventId,
            Guid PartId,
            decimal Price
        );

        public record UpdateEventPartDTO(
            decimal Price
        );

        public record GetEventPartDTO(
            Guid RepairEventId,
            Guid PartId,
            decimal Price
        );


        public record CreateEventTypeDTO(string Name);

        public record UpdateEventTypeDTO(string Name);

        public record GetEventTypeDTO(Guid Id, string Name);



        public record CreateFuelingEventDTO(
            Guid VehicleEventId,
            decimal Liters,
            decimal TotalPrice
        );

        public record UpdateFuelingEventDTO(
            decimal Liters,
            decimal TotalPrice
        );

        public record GetFuelingEventDTO(
            Guid Id,
            Guid VehicleEventId,
            decimal Liters,
            decimal TotalPrice
        );

        public record CreatePartDTO(string Name);

        public record UpdatePartDTO(string Name);

        public record GetPartDTO(Guid Id, string Name);



        public record CreateRepairEventDTO(
        Guid VehicleEventId,
        decimal LaborCost
    );

        public record UpdateRepairEventDTO(
            decimal LaborCost
        );

        public record GetRepairEventDTO(
            Guid Id,
            Guid VehicleEventId,
            decimal LaborCost
        );

        public record CreateServiceEventDTO(
            Guid VehicleEventId,
            Guid WorkshopId
        );

        public record UpdateServiceEventDTO(
            Guid WorkshopId
        );

        public record GetServiceEventDTO(
            Guid Id,
            Guid VehicleEventId,
            Guid WorkshopId
        );

        public record CreateVehicleEventDTO(
            Guid VehicleId,
            Guid EventTypeId,
            DateTime EventDate,
            int Mileage,
            string? Description
        );

        public record UpdateVehicleEventDTO(
            Guid EventTypeId,
            DateTime EventDate,
            int Mileage,
            string? Description
        );

        public record GetVehicleEventDTO(
            Guid Id,
            Guid VehicleId,
            Guid EventTypeId,
            DateTime EventDate,
            int Mileage,
            string? Description
        );

        public record CreateWorkshopDTO(string Name);

        public record UpdateWorkshopDTO(string Name);

        public record GetWorkshopDTO(Guid Id, string Name);

    }
}
