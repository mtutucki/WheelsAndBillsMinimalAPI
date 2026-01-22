using WheelsAndBillsAPI.Domain.Entities.Cost;
using WheelsAndBillsAPI.Endpoints.Auth;
using WheelsAndBillsAPI.Endpoints.Cost.Costs;
using WheelsAndBillsAPI.Endpoints.Cost.CostType;
using WheelsAndBillsAPI.Endpoints.Cost.RecurringCosts;

namespace WheelsAndBillsAPI.Endpoints.Cost
{
    public static class CostsEndpoints
    {
        public static IEndpointRouteBuilder MapCostsEndpoints(this IEndpointRouteBuilder app)
        {
            var costs = app
                .MapGroup("/costs")
                .WithTags("Costs")
                .RequireAuthorization();

            var costsTupe = app
                .MapGroup("/cost-type")
                .WithTags("Costs type")
                .RequireAuthorization();

            var recurringCost = app
                .MapGroup("/recurring-cost")
                .WithTags("Recurring Costs")
                .RequireAuthorization();

            costs.MapCreateCost();
            costs.MapUpdateCost();
            costs.MapDeleteCost();
            costs.MapGetCosts();
            costs.MapGetCostById();

            costsTupe.MapCreateCostType();
            costsTupe.MapUpdateCostType();
            costsTupe.MapDeleteCostType();
            costsTupe.MapGetCostTypes();
            costsTupe.MapGetCostTypeById();

            recurringCost.MapCreateRecurringCost();
            recurringCost.MapUpdateRecurringCost();
            recurringCost.MapDeleteRecurringCost();
            recurringCost.MapGetRecurringCosts();
            recurringCost.MapGetRecurringCostById();

            return app;
        }
    }
}
