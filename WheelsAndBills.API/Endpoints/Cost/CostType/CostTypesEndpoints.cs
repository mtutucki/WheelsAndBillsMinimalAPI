using static WheelsAndBills.Application.DTOs.Costs.CostsDTO;
using WheelsAndBills.Application.Features.Cost.CostTypes;

namespace WheelsAndBills.API.Endpoints.Cost.CostType
{
    public static class CostTypesEndpoints
    {

        public static RouteHandlerBuilder MapGetCostTypes(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                ICostTypesService costTypes,
                CancellationToken cancellationToken) =>
            {
                var types = await costTypes.GetAllAsync(cancellationToken);

                return Results.Ok(types);
            });
        }

        public static RouteHandlerBuilder MapGetCostTypeById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                ICostTypesService costTypes,
                CancellationToken cancellationToken) =>
            {
                var type = await costTypes.GetByIdAsync(id, cancellationToken);

                return type is null
                    ? Results.NotFound()
                    : Results.Ok(type);
            });
        }


        public static RouteHandlerBuilder MapCreateCostType(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateCostTypeDTO request,
                ICostTypesService costTypes,
                CancellationToken cancellationToken) =>
            {
                var result = await costTypes.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/cost-types/{result.Data!.Id}",
                    result.Data
                );
            });
        }


        public static RouteHandlerBuilder MapUpdateCostType(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateCostTypeDTO request,
                ICostTypesService costTypes,
                CancellationToken cancellationToken) =>
            {
                var result = await costTypes.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }


        public static RouteHandlerBuilder MapDeleteCostType(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                ICostTypesService costTypes,
                CancellationToken cancellationToken) =>
            {
                var result = await costTypes.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
