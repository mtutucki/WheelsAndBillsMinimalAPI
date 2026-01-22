using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WheelsAndBillsAPI.Domain.Entities.Vehicles;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Vehicles.Vehicles.UserVehicles
{
    public static class AddVehicle
    {
        public static RouteHandlerBuilder MapCreateMyVehicle(this RouteGroupBuilder group)
        {
            return group.MapPost("", [Authorize] async (
                CreateVehicleRequestDTO request,
                AppDbContext db,
                ClaimsPrincipal user) =>
            {
                var userId = Guid.Parse(
                    user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var vehicle = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Vin = request.Vin,
                    Year = request.Year,
                    BrandId = request.BrandId,
                    ModelId = request.ModelId,
                    TypeId = request.TypeId,
                    StatusId = request.StatusId
                };

                db.Vehicles.Add(vehicle);
                await db.SaveChangesAsync();

                return Results.Created($"/vehicles/{vehicle.Id}", vehicle.Id);
            });
        }
    }
}

