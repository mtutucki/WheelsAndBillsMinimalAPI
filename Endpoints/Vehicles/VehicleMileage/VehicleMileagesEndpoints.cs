using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Vehicles.VehicleMileage
{
    public static class VehicleMileagesEndpoints
    {
        public static RouteHandlerBuilder MapGetVehicleMileages(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var items = await db.VehicleMileage
                    .Select(m => new GetVehicleMileageDTO(
                        m.Id,
                        m.VehicleId,
                        m.Mileage,
                        m.Date
                    ))
                    .ToListAsync();

                return Results.Ok(items);
            });
        }

        public static RouteHandlerBuilder MapGetVehicleMileageById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var item = await db.VehicleMileage
                    .Where(m => m.Id == id)
                    .Select(m => new GetVehicleMileageDTO(
                        m.Id,
                        m.VehicleId,
                        m.Mileage,
                        m.Date
                    ))
                    .FirstOrDefaultAsync();

                return item is null
                    ? Results.NotFound()
                    : Results.Ok(item);
            });
        }

        public static RouteHandlerBuilder MapCreateVehicleMileage(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateVehicleMileageDTO request,
                AppDbContext db) =>
            {
                var vehicleExists = await db.Vehicles
                    .AnyAsync(v => v.Id == request.VehicleId);
                if (!vehicleExists)
                    return Results.BadRequest("Vehicle does not exist");

                var item = new Domain.Entities.Vehicles.VehicleMileage
                {
                    Id = Guid.NewGuid(),
                    VehicleId = request.VehicleId,
                    Mileage = request.Mileage,
                    Date = request.Date
                };

                db.VehicleMileage.Add(item);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/vehicle-mileages/{item.Id}",
                    new GetVehicleMileageDTO(
                        item.Id,
                        item.VehicleId,
                        item.Mileage,
                        item.Date
                    )
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateVehicleMileage(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateVehicleMileageDTO request,
                AppDbContext db) =>
            {
                var item = await db.VehicleMileage.FindAsync(id);
                if (item is null)
                    return Results.NotFound();

                item.Mileage = request.Mileage;
                item.Date = request.Date;

                await db.SaveChangesAsync();

                return Results.Ok(new GetVehicleMileageDTO(
                    item.Id,
                    item.VehicleId,
                    item.Mileage,
                    item.Date
                ));
            });
        }

        public static RouteHandlerBuilder MapDeleteVehicleMileage(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var item = await db.VehicleMileage.FindAsync(id);
                if (item is null)
                    return Results.NotFound();

                db.VehicleMileage.Remove(item);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }

        public static RouteHandlerBuilder MapCreateMyVehicleMileage(this RouteGroupBuilder app)
        {
            return app.MapPost("/my-mileage", async (
                CreateVehicleMileageDTO request,
                ClaimsPrincipal user,
                AppDbContext db) =>
            {
                var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdString);

                var vehicleExists = await db.Vehicles
                    .AnyAsync(v =>
                        v.Id == request.VehicleId &&
                        v.UserId == userId
                    );

                if (!vehicleExists)
                    return Results.BadRequest("Vehicle does not belong to user");

                var lastMileage = await db.VehicleMileage
                    .Where(x => x.VehicleId == request.VehicleId)
                    .OrderByDescending(x => x.Date)
                    .Select(x => x.Mileage)
                    .FirstOrDefaultAsync();

                if (lastMileage != 0 && request.Mileage <= lastMileage)
                    return Results.BadRequest(
                        $"Mileage must be greater than last value ({lastMileage} km)"
                    );

                var item = new Domain.Entities.Vehicles.VehicleMileage
                {
                    Id = Guid.NewGuid(),
                    VehicleId = request.VehicleId,
                    Mileage = request.Mileage,
                    Date = request.Date
                };

                db.VehicleMileage.Add(item);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/vehicle-mileages/{item.Id}",
                    new GetVehicleMileageDTO(
                        item.Id,
                        item.VehicleId,
                        item.Mileage,
                        item.Date
                    )
                );
            });
        }

        public static RouteHandlerBuilder MapDeleteMyVehicleMileage(this RouteGroupBuilder app)
        {
            return app.MapDelete("/my-mileages/{id:guid}", async (
                Guid id,
                ClaimsPrincipal user,
                AppDbContext db) =>
            {
                var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdString);

                var item = await db.VehicleMileage
                    .Include(vm => vm.Vehicle)
                    .FirstOrDefaultAsync(vm => vm.Id == id);

                if (item is null)
                    return Results.NotFound();

                if (item.Vehicle.UserId != userId)
                    return Results.Forbid();

                db.VehicleMileage.Remove(item);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }

    }
}
