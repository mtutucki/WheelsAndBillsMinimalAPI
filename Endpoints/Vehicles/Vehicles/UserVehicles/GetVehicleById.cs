using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Vehicles.Vehicles.UserVehicles
{
    public static class GetVehicleById
    {
        public static RouteHandlerBuilder MapGetUserVehicleById(this RouteGroupBuilder group)
        {
            return group.MapGet("/{id:guid}", [Authorize] async (
                Guid id,
                AppDbContext db,
                ClaimsPrincipal user) =>
            {
                var userId = Guid.Parse(
                    user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var vehicle = await db.Vehicles
                    .AsNoTracking()
                    .Where(v => v.Id == id && v.UserId == userId)
                    .Include(v => v.Brand)
                    .Include(v => v.Model)
                    .Include(v => v.Type)
                    .Include(v => v.Status)
                    .Select(v => new VehicleDetailsDTO(
                        v.Id,
                        v.Vin,
                        v.Year,

                        new LookupDTO(v.BrandId, v.Brand.Name),
                        new LookupDTO(v.ModelId, v.Model.Name),
                        new LookupDTO(v.TypeId, v.Type.Name),
                        new LookupDTO(v.StatusId, v.Status.Name),

                        db.VehicleMileage
                            .Where(m => m.VehicleId == v.Id)
                            .OrderByDescending(m => m.Date)
                            .Select(m => new VehicleMileageDTO(
                                m.Id,
                                m.Mileage,
                                m.Date
                            ))
                            .ToList(),

                        db.VehicleEvents
                            .Where(e => e.VehicleId == v.Id)
                            .Include(e => e.EventType)
                            .OrderByDescending(e => e.EventDate)
                            .Select(e => new VehicleEventDTO(
                                e.Id,
                                new LookupDTO(e.EventTypeId, e.EventType.Name),
                                e.EventDate,
                                e.Mileage,
                                e.Description
                            ))
                            .ToList(),

                        db.VehicleNotes
                            .Where(n => n.VehicleId == v.Id)
                            .OrderByDescending(n => n.CreatedAt)
                            .Select(n => new VehicleNoteDTO(
                                n.Id,
                                n.Content,
                                n.CreatedAt,
                                n.UserId
                            ))
                            .ToList()
                    ))
                    .FirstOrDefaultAsync();

                if (vehicle == null)
                    return Results.NotFound();

                return Results.Ok(vehicle);
            });
        }
    }
}
