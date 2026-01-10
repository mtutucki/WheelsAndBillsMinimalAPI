namespace WheelsAndBillsAPI.Domain.Entities.Vehicles
{
    public class VehicleMileage
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public int Mileage { get; set; }
        public DateTime Date { get; set; }

        public Vehicle Vehicle { get; set; } = null!;
    }
}
