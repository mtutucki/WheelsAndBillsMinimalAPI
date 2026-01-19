using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Vehicles
{
    public static class GetVehicles
    {
        public static IEndpointRouteBuilder MapGetVehicles(this IEndpointRouteBuilder app)
        {
            app.MapGet("/vehicles", [Authorize] async (
                AppDbContext db,
                ClaimsPrincipal user) =>
            {
                var userId = Guid.Parse(
                    user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                if ( userId == Guid.Empty)
                    return Results.Unauthorized();

                var vehicles = await db.Vehicles
                    .Where(v => v.UserId == userId)
                    .Select(v => new VehicleListItemResponse( 
                        v.Id,
                        v.Vin,
                        v.Year,
                        v.BrandId,
                        v.ModelId,
                        v.TypeId,
                        v.StatusId
                        ))
                    .ToListAsync();

                return Results.Ok(vehicles );
            });



            return app;
        }
        public record VehicleListItemResponse(
            Guid Id,
            string Vin,
            int Year,
            Guid BrandId,
            Guid ModelId,
            Guid TypeId,
            Guid StatusId
        );
    }
}
