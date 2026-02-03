using WheelsAndBills.Application.Features.Vehicles.VehicleStatuses;

namespace WheelsAndBills.API.Endpoints.Vehicles.VehicleStatus
{
    public static class VehicleStatusesEndpoints
    {
        public static RouteHandlerBuilder MapGetVehicleStatuses(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IVehicleStatusesService vehicleStatuses,
                CancellationToken cancellationToken) =>
            {
                var statuses = await vehicleStatuses.GetAllAsync(cancellationToken);

                return Results.Ok(statuses);
            });
        }

        public static RouteHandlerBuilder MapGetVehicleStatusById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IVehicleStatusesService vehicleStatuses,
                CancellationToken cancellationToken) =>
            {
                var status = await vehicleStatuses.GetByIdAsync(id, cancellationToken);

                return status is null
                    ? Results.NotFound()
                    : Results.Ok(status);
            });
        }

        public static RouteHandlerBuilder MapCreateVehicleStatus(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateVehicleStatusDTO request,
                IVehicleStatusesService vehicleStatuses,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleStatuses.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/vehicle-statuses/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateVehicleStatus(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateVehicleStatusDTO request,
                IVehicleStatusesService vehicleStatuses,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleStatuses.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteVehicleStatus(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IVehicleStatusesService vehicleStatuses,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleStatuses.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
