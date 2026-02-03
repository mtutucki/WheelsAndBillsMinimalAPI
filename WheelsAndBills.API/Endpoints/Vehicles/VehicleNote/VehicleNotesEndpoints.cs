using System.Security.Claims;
using WheelsAndBills.Application.Features.Vehicles.VehicleNotes;

namespace WheelsAndBills.API.Endpoints.Vehicles.VehicleNote
{
    public static class VehicleNotesEndpoints
    {
        public static RouteHandlerBuilder MapGetVehicleNotes(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IVehicleNotesService vehicleNotes,
                CancellationToken cancellationToken) =>
            {
                var notes = await vehicleNotes.GetAllAsync(cancellationToken);

                return Results.Ok(notes);
            });
        }

        public static RouteHandlerBuilder MapGetVehicleNoteById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IVehicleNotesService vehicleNotes,
                CancellationToken cancellationToken) =>
            {
                var note = await vehicleNotes.GetByIdAsync(id, cancellationToken);

                return note is null
                    ? Results.NotFound()
                    : Results.Ok(note);
            });
        }

        public static RouteHandlerBuilder MapCreateVehicleNote(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateVehicleNoteDTO request,
                IVehicleNotesService vehicleNotes,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleNotes.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/vehicle-notes/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateVehicleNote(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateVehicleNoteDTO request,
                IVehicleNotesService vehicleNotes,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleNotes.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteVehicleNote(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IVehicleNotesService vehicleNotes,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleNotes.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }

        public static RouteHandlerBuilder MapDeleteMyVehicleNote(this RouteGroupBuilder app)
        {
            return app.MapDelete("/my-notes/{id:guid}", async (
                Guid id,
                ClaimsPrincipal user,
                IVehicleNotesService vehicleNotes,
                CancellationToken cancellationToken) =>
            {
                var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdString);

                var result = await vehicleNotes.DeleteForUserAsync(userId, id, cancellationToken);
                if (!result.Success)
                    return result.Error == "Forbidden"
                        ? Results.Forbid()
                        : Results.NotFound();

                return Results.NoContent();
            });
        }

        public static RouteHandlerBuilder MapCreateMyVehicleNote(this RouteGroupBuilder app)
        {
            return app.MapPost("/my-notes", async (
                CreateMyVehicleNoteDTO request,
                ClaimsPrincipal user,
                IVehicleNotesService vehicleNotes,
                CancellationToken cancellationToken) =>
            {
                var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdString is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdString);

                var result = await vehicleNotes.CreateForUserAsync(userId, request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/vehicle-notes/{result.Data!.Id}",
                    result.Data
                );
            });
        }
    }
}
