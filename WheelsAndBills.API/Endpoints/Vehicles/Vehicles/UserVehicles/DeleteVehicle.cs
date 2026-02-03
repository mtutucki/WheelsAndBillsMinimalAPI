using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WheelsAndBills.Application.Features.Vehicles.UserVehicles;

namespace WheelsAndBills.API.Endpoints.Vehicles.Vehicles.UserVehicles
{
    public static class DeleteVehicle
    {
        public static RouteHandlerBuilder MapDeleteMyVehicle(this RouteGroupBuilder group)
        {
            return group.MapDelete("/{vehicleId:guid}", [Authorize] async (
                Guid vehicleId,
                ClaimsPrincipal user,
                IUserVehiclesService userVehicles,
                CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(
                    user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var deleted = await userVehicles.DeleteVehicleAsync(userId, vehicleId, cancellationToken);

                return deleted ? Results.NoContent() : Results.NotFound();
            });
        }
    }
}
