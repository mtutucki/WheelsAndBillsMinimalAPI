using System.Security.Claims;
using WheelsAndBills.Application.Features.Vehicles.VehicleMileage;

namespace WheelsAndBills.API.Endpoints.Vehicles.VehicleMileage
{
    public static class VehicleMileagesEndpoints
    {
        public static RouteHandlerBuilder MapGetVehicleMileages(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IVehicleMileageService vehicleMileage,
                CancellationToken cancellationToken) =>
            {
                var items = await vehicleMileage.GetAllAsync(cancellationToken);

                return Results.Ok(items);
            });
        }

        public static RouteHandlerBuilder MapGetVehicleMileageById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IVehicleMileageService vehicleMileage,
                CancellationToken cancellationToken) =>
            {
                var item = await vehicleMileage.GetByIdAsync(id, cancellationToken);

                return item is null
                    ? Results.NotFound()
                    : Results.Ok(item);
            });
        }

        public static RouteHandlerBuilder MapCreateVehicleMileage(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateVehicleMileageDTO request,
                IVehicleMileageService vehicleMileage,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleMileage.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/vehicle-mileages/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateVehicleMileage(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateVehicleMileageDTO request,
                IVehicleMileageService vehicleMileage,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleMileage.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteVehicleMileage(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IVehicleMileageService vehicleMileage,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleMileage.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }

        public static RouteHandlerBuilder MapCreateMyVehicleMileage(this RouteGroupBuilder app)
        {
            return app.MapPost("/my-mileage", async (
                CreateVehicleMileageDTO request,
                ClaimsPrincipal user,
                IVehicleMileageService vehicleMileage,
                CancellationToken cancellationToken) =>
            {
                var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdString);

                var result = await vehicleMileage.CreateForUserAsync(userId, request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/vehicle-mileages/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapDeleteMyVehicleMileage(this RouteGroupBuilder app)
        {
            return app.MapDelete("/my-mileages/{id:guid}", async (
                Guid id,
                ClaimsPrincipal user,
                IVehicleMileageService vehicleMileage,
                CancellationToken cancellationToken) =>
            {
                var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdString);

                var result = await vehicleMileage.DeleteForUserAsync(userId, id, cancellationToken);
                if (!result.Success)
                    return result.Error == "Forbidden"
                        ? Results.Forbid()
                        : Results.NotFound();

                return Results.NoContent();
            });
        }

    }
}
