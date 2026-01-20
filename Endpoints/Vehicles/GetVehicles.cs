using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Endpoints.Vehicles.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Vehicles
{
    public static class GetVehicles
    {
        public static RouteHandlerBuilder MapGetVehicles(this RouteGroupBuilder group)
        {
            return group.MapGet("/", [Authorize] async (
                AppDbContext db,
                ClaimsPrincipal user) =>
            {
                var userId = Guid.Parse(
                    user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var vehicles = await db.Vehicles
                    .Where(v => v.UserId == userId)
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
