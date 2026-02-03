using WheelsAndBills.Application.Features.Vehicles.VehiclesAdmin;

namespace WheelsAndBills.API.Endpoints.Vehicles.Vehicles
{
    public static class VehiclesAdminEndpoints
    {
        public static RouteHandlerBuilder MapGetVehicles(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IVehiclesAdminService vehiclesAdmin,
                CancellationToken cancellationToken) =>
            {
                var vehicles = await vehiclesAdmin.GetAllAsync(cancellationToken);

                return Results.Ok(vehicles);
            });
        }

        public static RouteHandlerBuilder MapGetVehicleById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IVehiclesAdminService vehiclesAdmin,
                CancellationToken cancellationToken) =>
            {
                var vehicle = await vehiclesAdmin.GetByIdAsync(id, cancellationToken);

                return vehicle is null
                    ? Results.NotFound()
                    : Results.Ok(vehicle);
            });
        }

        public static RouteHandlerBuilder MapCreateVehicle(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateVehicleDTO request,
                IVehiclesAdminService vehiclesAdmin,
                CancellationToken cancellationToken) =>
            {
                var result = await vehiclesAdmin.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/vehicles/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateVehicle(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateVehicleDTO request,
                IVehiclesAdminService vehiclesAdmin,
                CancellationToken cancellationToken) =>
            {
                var result = await vehiclesAdmin.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteVehicle(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IVehiclesAdminService vehiclesAdmin,
                CancellationToken cancellationToken) =>
            {
                var result = await vehiclesAdmin.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
