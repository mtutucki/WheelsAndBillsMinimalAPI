using WheelsAndBills.Application.Features.Vehicles.VehicleTypes;

namespace WheelsAndBills.API.Endpoints.Vehicles.VehicleType
{
    public static class VehicleTypesEndpoints
    {
        public static RouteHandlerBuilder MapGetVehicleTypes(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IVehicleTypesService vehicleTypes,
                CancellationToken cancellationToken) =>
            {
                var types = await vehicleTypes.GetAllAsync(cancellationToken);

                return Results.Ok(types);
            });
        }

        public static RouteHandlerBuilder MapGetVehicleTypeById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IVehicleTypesService vehicleTypes,
                CancellationToken cancellationToken) =>
            {
                var type = await vehicleTypes.GetByIdAsync(id, cancellationToken);

                return type is null
                    ? Results.NotFound()
                    : Results.Ok(type);
            });
        }

        public static RouteHandlerBuilder MapCreateVehicleType(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateVehicleTypeDTO request,
                IVehicleTypesService vehicleTypes,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleTypes.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/vehicle-types/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateVehicleType(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateVehicleTypeDTO request,
                IVehicleTypesService vehicleTypes,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleTypes.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteVehicleType(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IVehicleTypesService vehicleTypes,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleTypes.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
