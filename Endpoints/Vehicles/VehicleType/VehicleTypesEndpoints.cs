using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Vehicles.VehicleType
{
    public static class VehicleTypesEndpoints
    {
        public static RouteHandlerBuilder MapGetVehicleTypes(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var types = await db.VehicleTypes
                    .OrderBy(t => t.Name)
                    .Select(t => new GetVehicleTypeDTO(t.Id, t.Name))
                    .ToListAsync();

                return Results.Ok(types);
            });
        }

        public static RouteHandlerBuilder MapGetVehicleTypeById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var type = await db.VehicleTypes
                    .Where(t => t.Id == id)
                    .Select(t => new GetVehicleTypeDTO(t.Id, t.Name))
                    .FirstOrDefaultAsync();

                return type is null
                    ? Results.NotFound()
                    : Results.Ok(type);
            });
        }

        public static RouteHandlerBuilder MapCreateVehicleType(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateVehicleTypeDTO request,
                AppDbContext db) =>
            {
                var exists = await db.VehicleTypes.AnyAsync(t => t.Name == request.Name);
                if (exists)
                    return Results.BadRequest("VehicleType already exists");

                var type = new Domain.Entities.Vehicles.VehicleType
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name
                };

                db.VehicleTypes.Add(type);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/vehicle-types/{type.Id}",
                    new GetVehicleTypeDTO(type.Id, type.Name)
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateVehicleType(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateVehicleTypeDTO request,
                AppDbContext db) =>
            {
                var type = await db.VehicleTypes.FindAsync(id);
                if (type is null)
                    return Results.NotFound();

                var exists = await db.VehicleTypes
                    .AnyAsync(t => t.Name == request.Name && t.Id != id);
                if (exists)
                    return Results.BadRequest("VehicleType already exists");

                type.Name = request.Name;
                await db.SaveChangesAsync();

                return Results.Ok(new GetVehicleTypeDTO(type.Id, type.Name));
            });
        }

        public static RouteHandlerBuilder MapDeleteVehicleType(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var type = await db.VehicleTypes.FindAsync(id);
                if (type is null)
                    return Results.NotFound();

                db.VehicleTypes.Remove(type);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
