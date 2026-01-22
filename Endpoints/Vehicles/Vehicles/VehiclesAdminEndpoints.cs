using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Vehicles.Vehicles
{
    public static class VehiclesAdminEndpoints
    {
        public static RouteHandlerBuilder MapGetVehicles(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var vehicles = await db.Vehicles
                    .Select(v => new GetVehicleDTO(
                        v.Id,
                        v.UserId,
                        v.Vin,
                        v.Year,
                        v.BrandId,
                        v.ModelId,
                        v.TypeId,
                        v.StatusId
                    ))
                    .ToListAsync();

                return Results.Ok(vehicles);
            });
        }

        public static RouteHandlerBuilder MapGetVehicleById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var vehicle = await db.Vehicles
                    .Where(v => v.Id == id)
                    .Select(v => new GetVehicleDTO(
                        v.Id,
                        v.UserId,
                        v.Vin,
                        v.Year,
                        v.BrandId,
                        v.ModelId,
                        v.TypeId,
                        v.StatusId
                    ))
                    .FirstOrDefaultAsync();

                return vehicle is null
                    ? Results.NotFound()
                    : Results.Ok(vehicle);
            });
        }

        public static RouteHandlerBuilder MapCreateVehicle(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateVehicleDTO request,
                AppDbContext db) =>
            {
                var userExists = await db.Users.AnyAsync(u => u.Id == request.UserId);
                if (!userExists)
                    return Results.BadRequest("User does not exist");

                var vinExists = await db.Vehicles.AnyAsync(v => v.Vin == request.Vin);
                if (vinExists)
                    return Results.BadRequest("Vehicle with this VIN already exists");

                var vehicle = new Domain.Entities.Vehicles.Vehicle
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    Vin = request.Vin,
                    Year = request.Year,
                    BrandId = request.BrandId,
                    ModelId = request.ModelId,
                    TypeId = request.TypeId,
                    StatusId = request.StatusId
                };

                db.Vehicles.Add(vehicle);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/vehicles/{vehicle.Id}",
                    new GetVehicleDTO(
                        vehicle.Id,
                        vehicle.UserId,
                        vehicle.Vin,
                        vehicle.Year,
                        vehicle.BrandId,
                        vehicle.ModelId,
                        vehicle.TypeId,
                        vehicle.StatusId
                    )
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateVehicle(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateVehicleDTO request,
                AppDbContext db) =>
            {
                var vehicle = await db.Vehicles.FindAsync(id);
                if (vehicle is null)
                    return Results.NotFound();

                var vinExists = await db.Vehicles
                    .AnyAsync(v => v.Vin == request.Vin && v.Id != id);
                if (vinExists)
                    return Results.BadRequest("Vehicle with this VIN already exists");

                vehicle.Vin = request.Vin;
                vehicle.Year = request.Year;
                vehicle.BrandId = request.BrandId;
                vehicle.ModelId = request.ModelId;
                vehicle.TypeId = request.TypeId;
                vehicle.StatusId = request.StatusId;

                await db.SaveChangesAsync();

                return Results.Ok(new GetVehicleDTO(
                    vehicle.Id,
                    vehicle.UserId,
                    vehicle.Vin,
                    vehicle.Year,
                    vehicle.BrandId,
                    vehicle.ModelId,
                    vehicle.TypeId,
                    vehicle.StatusId
                ));
            });
        }

        public static RouteHandlerBuilder MapDeleteVehicle(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var vehicle = await db.Vehicles.FindAsync(id);
                if (vehicle is null)
                    return Results.NotFound();

                db.Vehicles.Remove(vehicle);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
