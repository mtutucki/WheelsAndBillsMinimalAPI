
namespace WheelsAndBillsAPI.Domain.Entities.Vehicles
{

    public class VehicleModel
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public string Name { get; set; } = null!;

        public VehicleBrand Brand { get; set; } = null!;
    }
}



