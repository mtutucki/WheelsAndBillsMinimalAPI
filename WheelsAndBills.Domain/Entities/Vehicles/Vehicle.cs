
using WheelsAndBills.Domain.Entities.Auth;
using WheelsAndBills.Domain.Entities.Events;

namespace WheelsAndBills.Domain.Entities.Vehicles
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string Vin { get; set; } = null!;
        public int Year { get; set; }

        public Guid BrandId { get; set; }
        public Guid ModelId { get; set; }
        public Guid TypeId { get; set; }
        public Guid StatusId { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public VehicleBrand Brand { get; set; } = null!;
        public VehicleModel Model { get; set; } = null!;
        public VehicleType Type { get; set; } = null!;
        public VehicleStatus Status { get; set; } = null!;

        public ICollection<VehicleEvent> Events { get; set; } = new List<VehicleEvent>();
        public ICollection<VehicleMileage> Mileages { get; set; } = new List<VehicleMileage>();
    }

}
