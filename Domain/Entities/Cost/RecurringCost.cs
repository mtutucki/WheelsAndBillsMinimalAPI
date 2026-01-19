using WheelsAndBillsAPI.Domain.Entities.Vehicles;

namespace WheelsAndBillsAPI.Domain.Entities.Cost
{
    public class RecurringCost
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public Guid CostTypeId { get; set; }
        public decimal Amount { get; set; }
        public int IntervalMonths { get; set; }

        public Vehicle Vehicle { get; set; } = null!;
    }

}
