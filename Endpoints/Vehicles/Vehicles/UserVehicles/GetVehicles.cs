using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Vehicles.Vehicles.UserVehicles
{
    public static class GetVehicles
    {
        public static RouteHandlerBuilder MapGetUserVehicles(this RouteGroupBuilder group)
        {
            return group.MapGet("", [Authorize] async (
                AppDbContext db,
                ClaimsPrincipal user) =>
            {
                var userId = Guid.Parse(
                    user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var vehicles = await db.Vehicles
                    .Where(v => v.UserId == userId && v.StatusId != Guid.Parse("85C30BAB-7FA3-4124-BE5D-1E220CACE01F"))
                    .OrderByDescending(v => v.Status.Name == "Aktywny")
                    .ThenBy(v => v.Brand.Name)
                    .Include(v => v.Brand)
                    .Include(v => v.Model)
                    .Include(v => v.Type)
                    .Include(v => v.Status)
                    .Select(v => new GetVehiclesByUserDTO(
                        v.Id,
                        v.Vin,
                        v.Year,

                        new LookupDTO(v.BrandId, v.Brand.Name),
                        new LookupDTO(v.ModelId, v.Model.Name),
                        new LookupDTO(v.TypeId, v.Type.Name),
                        new LookupDTO(v.StatusId, v.Status.Name)
                    ))
                    .ToListAsync();

                return Results.Ok(vehicles);
            });
        }
    }
}
