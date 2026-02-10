using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WheelsAndBills.Application.Features.Vehicles.UserVehicles;

namespace WheelsAndBills.API.Endpoints.Vehicles.Vehicles.UserVehicles
{
    public static class GetVehicleById
    {
        public static RouteHandlerBuilder MapGetUserVehicleById(this RouteGroupBuilder group)
        {
            return group.MapGet("/{id:guid}", [Authorize] async (
                Guid id,
                HttpRequest request,
                ClaimsPrincipal user,
                IUserVehiclesService userVehicles,
                CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(
                    user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var vehicle = await userVehicles.GetUserVehicleByIdAsync(userId, id, cancellationToken);

                if (vehicle == null)
                    return Results.NotFound();

                var baseUrl = $"{request.Scheme}://{request.Host}";
                if (!string.IsNullOrWhiteSpace(vehicle.AvatarUrl) &&
                    vehicle.AvatarUrl.StartsWith("/"))
                {
                    vehicle = vehicle with { AvatarUrl = $"{baseUrl}{vehicle.AvatarUrl}" };
                }

                return Results.Ok(vehicle);
            });
        }
    }
}
