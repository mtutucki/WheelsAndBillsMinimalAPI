using WheelsAndBills.Domain.Entities.Cost;
using WheelsAndBills.API.Endpoints.Auth;
using WheelsAndBills.API.Endpoints.Cost.Costs;
using WheelsAndBills.API.Endpoints.Cost.CostType;
using WheelsAndBills.API.Endpoints.Cost.RecurringCosts;

namespace WheelsAndBills.API.Endpoints.Cost
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
