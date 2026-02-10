using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WheelsAndBills.Application.Features.Vehicles.UserVehicles;

namespace WheelsAndBills.API.Endpoints.Vehicles.Vehicles.UserVehicles
{
    public static class GetVehicles
    {
        public static RouteHandlerBuilder MapGetUserVehicles(this RouteGroupBuilder group)
        {
            return group.MapGet("", [Authorize] async (
                HttpRequest request,
                ClaimsPrincipal user,
                IUserVehiclesService userVehicles,
                CancellationToken cancellationToken) =>
            {
                var userId = Guid.Parse(
                    user.FindFirstValue(ClaimTypes.NameIdentifier)!);

                if (userId == Guid.Empty)
                    return Results.Unauthorized();

                var vehicles = await userVehicles.GetUserVehiclesAsync(userId, cancellationToken);
                var baseUrl = $"{request.Scheme}://{request.Host}";
                var result = vehicles
                    .Select(v => v.AvatarUrl is { Length: > 0 } && v.AvatarUrl.StartsWith("/")
                        ? v with { AvatarUrl = $"{baseUrl}{v.AvatarUrl}" }
                        : v)
                    .ToList();

                return Results.Ok(result);
            });
        }
    }
}
