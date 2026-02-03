using WheelsAndBills.Application.Features.Vehicles.VehicleModels;

namespace WheelsAndBills.API.Endpoints.Vehicles.VehicleModel
{
    public static class VehicleModelsEndpoints
    {
        public static RouteHandlerBuilder MapGetVehicleModels(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IVehicleModelsService vehicleModels,
                CancellationToken cancellationToken) =>
            {
                var models = await vehicleModels.GetAllAsync(cancellationToken);

                return Results.Ok(models);
            });
        }

        public static RouteHandlerBuilder MapGetVehicleModelById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IVehicleModelsService vehicleModels,
                CancellationToken cancellationToken) =>
            {
                var model = await vehicleModels.GetByIdAsync(id, cancellationToken);

                return model is null
                    ? Results.NotFound()
                    : Results.Ok(model);
            });
        }

        public static RouteHandlerBuilder MapCreateVehicleModel(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateVehicleModelDTO request,
                IVehicleModelsService vehicleModels,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleModels.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/vehicle-models/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateVehicleModel(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateVehicleModelDTO request,
                IVehicleModelsService vehicleModels,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleModels.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteVehicleModel(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IVehicleModelsService vehicleModels,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleModels.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
