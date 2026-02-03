using static WheelsAndBills.Application.DTOs.Costs.CostsDTO;
using WheelsAndBills.Application.Features.Cost.RecurringCosts;

namespace WheelsAndBills.API.Endpoints.Cost.RecurringCosts
{
    public static class RecurringCostsEndpoints
    {

        public static RouteHandlerBuilder MapGetRecurringCosts(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IRecurringCostsService recurringCosts,
                CancellationToken cancellationToken) =>
            {
                var costs = await recurringCosts.GetAllAsync(cancellationToken);

                return Results.Ok(costs);
            });
        }

        public static RouteHandlerBuilder MapGetRecurringCostById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IRecurringCostsService recurringCosts,
                CancellationToken cancellationToken) =>
            {
                var cost = await recurringCosts.GetByIdAsync(id, cancellationToken);

                return cost is null
                    ? Results.NotFound()
                    : Results.Ok(cost);
            });
        }


        public static RouteHandlerBuilder MapCreateRecurringCost(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateRecurringCostDTO request,
                IRecurringCostsService recurringCosts,
                CancellationToken cancellationToken) =>
            {
                var result = await recurringCosts.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/recurring-costs/{result.Data!.Id}",
                    result.Data
                );
            });
        }


        public static RouteHandlerBuilder MapUpdateRecurringCost(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateRecurringCostDTO request,
                IRecurringCostsService recurringCosts,
                CancellationToken cancellationToken) =>
            {
                var result = await recurringCosts.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }


        public static RouteHandlerBuilder MapDeleteRecurringCost(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IRecurringCostsService recurringCosts,
                CancellationToken cancellationToken) =>
            {
                var result = await recurringCosts.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
