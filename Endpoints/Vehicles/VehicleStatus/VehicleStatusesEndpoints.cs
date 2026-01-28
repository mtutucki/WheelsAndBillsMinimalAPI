using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Vehicles.VehicleStatus
{
    public static class VehicleStatusesEndpoints
    {
        public static RouteHandlerBuilder MapGetVehicleStatuses(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var statuses = await db.VehicleStatuses
                    .Where(s => s.Id != Guid.Parse("85C30BAB-7FA3-4124-BE5D-1E220CACE01F"))
                    .OrderBy(s => s.Name)
                    .Select(s => new GetVehicleStatusDTO(s.Id, s.Name))
                    .ToListAsync();

                return Results.Ok(statuses);
            });
        }

        public static RouteHandlerBuilder MapGetVehicleStatusById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var status = await db.VehicleStatuses
                    .Where(s => s.Id == id)
                    .Select(s => new GetVehicleStatusDTO(s.Id, s.Name))
                    .FirstOrDefaultAsync();

                return status is null
                    ? Results.NotFound()
                    : Results.Ok(status);
            });
        }

        public static RouteHandlerBuilder MapCreateVehicleStatus(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateVehicleStatusDTO request,
                AppDbContext db) =>
            {
                var exists = await db.VehicleStatuses
                    .AnyAsync(s => s.Name == request.Name);
                if (exists)
                    return Results.BadRequest("VehicleStatus already exists");

                var status = new Domain.Entities.Vehicles.VehicleStatus
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name
                };

                db.VehicleStatuses.Add(status);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/vehicle-statuses/{status.Id}",
                    new GetVehicleStatusDTO(status.Id, status.Name)
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateVehicleStatus(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateVehicleStatusDTO request,
                AppDbContext db) =>
            {
                var status = await db.VehicleStatuses.FindAsync(id);
                if (status is null)
                    return Results.NotFound();

                var exists = await db.VehicleStatuses
                    .AnyAsync(s => s.Name == request.Name && s.Id != id);
                if (exists)
                    return Results.BadRequest("VehicleStatus already exists");

                status.Name = request.Name;
                await db.SaveChangesAsync();

                return Results.Ok(new GetVehicleStatusDTO(status.Id, status.Name));
            });
        }

        public static RouteHandlerBuilder MapDeleteVehicleStatus(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var status = await db.VehicleStatuses.FindAsync(id);
                if (status is null)
                    return Results.NotFound();

                db.VehicleStatuses.Remove(status);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
