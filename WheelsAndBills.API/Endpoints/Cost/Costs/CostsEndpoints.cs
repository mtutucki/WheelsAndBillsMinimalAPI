using static WheelsAndBills.Application.DTOs.Costs.CostsDTO;
using WheelsAndBills.Application.Features.Cost.Costs;

namespace WheelsAndBills.API.Endpoints.Cost.Costs
{
    public static class CostsEndpoints
    {

        public static RouteHandlerBuilder MapGetCosts(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                ICostsService costsService,
                CancellationToken cancellationToken) =>
            {
                var costs = await costsService.GetAllAsync(cancellationToken);

                return Results.Ok(costs);
            });
        }

        public static RouteHandlerBuilder MapGetCostById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                ICostsService costsService,
                CancellationToken cancellationToken) =>
            {
                var cost = await costsService.GetByIdAsync(id, cancellationToken);

                return cost is null
                    ? Results.NotFound()
                    : Results.Ok(cost);
            });
        }


        public static RouteHandlerBuilder MapCreateCost(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateCostDTO request,
                ICostsService costsService,
                CancellationToken cancellationToken) =>
            {
                var result = await costsService.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/costs/{result.Data!.Id}",
                    result.Data
                );
            });
        }


        public static RouteHandlerBuilder MapUpdateCost(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateCostDTO request,
                ICostsService costsService,
                CancellationToken cancellationToken) =>
            {
                var result = await costsService.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }


        public static RouteHandlerBuilder MapDeleteCost(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                ICostsService costsService,
                CancellationToken cancellationToken) =>
            {
                var result = await costsService.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
