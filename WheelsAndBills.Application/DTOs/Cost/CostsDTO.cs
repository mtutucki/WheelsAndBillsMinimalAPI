namespace WheelsAndBills.Application.DTOs.Costs
{
    public class CostsDTO
    {
        public record CreateCostDTO(
            Guid VehicleEventId,
            Guid CostTypeId,
            decimal Amount
        );

        public record UpdateCostDTO(
            Guid CostTypeId,
            decimal Amount
        );

        public record GetCostDTO(
            Guid Id,
            Guid VehicleEventId,
            Guid CostTypeId,
            decimal Amount
        );


        public record CreateCostTypeDTO(string Name);

        public record UpdateCostTypeDTO(string Name);

        public record GetCostTypeDTO(Guid Id, string Name);


        public record CreateRecurringCostDTO(
            Guid VehicleId,
            Guid CostTypeId,
            decimal Amount,
            int IntervalMonths
        );

        public record UpdateRecurringCostDTO(
            Guid CostTypeId,
            decimal Amount,
            int IntervalMonths
        );

        public record GetRecurringCostDTO(
            Guid Id,
            Guid VehicleId,
            Guid CostTypeId,
            decimal Amount,
            int IntervalMonths
        );
    }
}
