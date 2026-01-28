using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Vehicles.Vehicles.UserVehicles
{
    public static class DeleteVehicle
    {
        public static RouteHandlerBuilder MapDeleteMyVehicle(this RouteGroupBuilder group)
        {
            return group.MapDelete("/{vehicleId:guid}", [Authorize] async (
                Guid vehicleId,
                AppDbContext db,
                ClaimsPrincipal user) =>
            {
                var userId = Guid.Parse(
                    user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var vehicle = await db.Vehicles
                    .FirstOrDefaultAsync(v =>
                        v.Id == vehicleId &&
                        v.UserId == userId);

                if (vehicle is null)
                    return Results.NotFound();

                vehicle.StatusId = Guid.Parse("85C30BAB-7FA3-4124-BE5D-1E220CACE01F");

                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
