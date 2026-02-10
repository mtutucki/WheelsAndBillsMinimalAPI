using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WheelsAndBills.Application.DTOs.Vehicles;
using WheelsAndBills.Application.Features.Vehicles.UserVehicles;

namespace WheelsAndBills.API.Endpoints.Vehicles.Vehicles.UserVehicles
{
    public static class UpdateVehicle
    {
        public static RouteHandlerBuilder MapUpdateMyVehicle(this RouteGroupBuilder group)
        {
            return group.MapPut("/{id:guid}", [Authorize] async (
                Guid id,
                UpdateMyVehicleDTO request,
                ClaimsPrincipal user,
                IUserVehiclesService userVehicles,
                CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(
                    user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var updated = await userVehicles.UpdateVehicleAsync(userId, id, request, cancellationToken);

                return updated ? Results.NoContent() : Results.NotFound();
            });
        }
    }
}
