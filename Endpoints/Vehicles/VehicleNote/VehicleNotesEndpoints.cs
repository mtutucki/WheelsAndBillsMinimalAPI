using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Vehicles.VehicleNote
{
    public static class VehicleNotesEndpoints
    {
        public static RouteHandlerBuilder MapGetVehicleNotes(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var notes = await db.VehicleNotes
                    .Select(n => new GetVehicleNoteDTO(
                        n.Id,
                        n.VehicleId,
                        n.UserId,
                        n.Content,
                        n.CreatedAt
                    ))
                    .ToListAsync();

                return Results.Ok(notes);
            });
        }

        public static RouteHandlerBuilder MapGetVehicleNoteById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var note = await db.VehicleNotes
                    .Where(n => n.Id == id)
                    .Select(n => new GetVehicleNoteDTO(
                        n.Id,
                        n.VehicleId,
                        n.UserId,
                        n.Content,
                        n.CreatedAt
                    ))
                    .FirstOrDefaultAsync();

                return note is null
                    ? Results.NotFound()
                    : Results.Ok(note);
            });
        }

        public static RouteHandlerBuilder MapCreateVehicleNote(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateVehicleNoteDTO request,
                AppDbContext db) =>
            {
                var vehicleExists = await db.Vehicles
                    .AnyAsync(v => v.Id == request.VehicleId);
                if (!vehicleExists)
                    return Results.BadRequest("Vehicle does not exist");

                var userExists = await db.Users
                    .AnyAsync(u => u.Id == request.UserId);
                if (!userExists)
                    return Results.BadRequest("User does not exist");

                var note = new Domain.Entities.Vehicles.VehicleNote
                {
                    Id = Guid.NewGuid(),
                    VehicleId = request.VehicleId,
                    UserId = request.UserId,
                    Content = request.Content,
                    CreatedAt = DateTime.UtcNow
                };

                db.VehicleNotes.Add(note);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/vehicle-notes/{note.Id}",
                    new GetVehicleNoteDTO(
                        note.Id,
                        note.VehicleId,
                        note.UserId,
                        note.Content,
                        note.CreatedAt
                    )
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateVehicleNote(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateVehicleNoteDTO request,
                AppDbContext db) =>
            {
                var note = await db.VehicleNotes.FindAsync(id);
                if (note is null)
                    return Results.NotFound();

                note.Content = request.Content;
                await db.SaveChangesAsync();

                return Results.Ok(new GetVehicleNoteDTO(
                    note.Id,
                    note.VehicleId,
                    note.UserId,
                    note.Content,
                    note.CreatedAt
                ));
            });
        }

        public static RouteHandlerBuilder MapDeleteVehicleNote(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var note = await db.VehicleNotes.FindAsync(id);
                if (note is null)
                    return Results.NotFound();

                db.VehicleNotes.Remove(note);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }

        public static RouteHandlerBuilder MapDeleteMyVehicleNote(this RouteGroupBuilder app)
        {
            return app.MapDelete("/my-notes/{id:guid}", async (
                Guid id,
                ClaimsPrincipal user,
                AppDbContext db) =>
            {
                var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdString);

                var item = await db.VehicleNotes
                    .Include(vm => vm.Vehicle)
                    .FirstOrDefaultAsync(vm => vm.Id == id);

                if (item is null)
                    return Results.NotFound();

                if (item.Vehicle.UserId != userId)
                    return Results.Forbid();

                db.VehicleNotes.Remove(item);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }

        public static RouteHandlerBuilder MapCreateMyVehicleNote(this RouteGroupBuilder app)
        {
            return app.MapPost("/my-notes", async (
                CreateMyVehicleNoteDTO request,
                ClaimsPrincipal user,
                AppDbContext db) =>
            {
                var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdString);

                var vehicleExists = await db.Vehicles
                    .AnyAsync(v => v.Id == request.VehicleId && v.UserId == userId);

                if (!vehicleExists)
                    return Results.BadRequest("Vehicle does not belong to user");

                var note = new Domain.Entities.Vehicles.VehicleNote
                {
                    Id = Guid.NewGuid(),
                    VehicleId = request.VehicleId,
                    UserId = userId,
                    Content = request.Content,
                    CreatedAt = DateTime.UtcNow
                };

                db.VehicleNotes.Add(note);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/vehicle-notes/{note.Id}",
                    new GetVehicleNoteDTO(
                        note.Id,
                        note.VehicleId,
                        note.UserId,
                        note.Content,
                        note.CreatedAt
                    )
                );
            });
        }
    }
}
