using Domain.Entities.Events;

namespace WheelsAndBillsAPI.Domain.Entities.Cost
{
    public class Cost
    {
        public Guid Id { get; set; }
        public Guid VehicleEventId { get; set; }
        public Guid CostTypeId { get; set; }
        public decimal Amount { get; set; }

        public VehicleEvent VehicleEvent { get; set; } = null!;
        public CostType CostType { get; set; } = null!;
    }

}
