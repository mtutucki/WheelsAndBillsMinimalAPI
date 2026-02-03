using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WheelsAndBills.Application.Features.Vehicles.UserVehicles;

namespace WheelsAndBills.API.Endpoints.Vehicles.Vehicles.UserVehicles
{
    public static class AddVehicle
    {
        public static RouteHandlerBuilder MapCreateMyVehicle(this RouteGroupBuilder group)
        {
            return group.MapPost("", [Authorize] async (
                CreateVehicleRequestDTO request,
                ClaimsPrincipal user,
                IUserVehiclesService userVehicles,
                CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(
                    user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var vehicleId = await userVehicles.CreateVehicleAsync(userId, request, cancellationToken);

                return Results.Created($"/vehicles/{vehicleId}", vehicleId);
            });
        }
    }
}

