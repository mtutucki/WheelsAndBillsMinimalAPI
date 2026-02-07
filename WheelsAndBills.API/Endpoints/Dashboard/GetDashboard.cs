using System.Security.Claims;
using WheelsAndBills.Application.Features.Dashboard;

namespace WheelsAndBills.API.Endpoints.Dashboard
{
    public static class GetDashboard
    {
        public static RouteHandlerBuilder MapGetDashboard(this RouteGroupBuilder app)
        {
            return app.MapGet("/", async (
                ClaimsPrincipal user,
                IDashboardService dashboardService,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();

                var result = await dashboardService.GetForUserAsync(userId, cancellationToken);
                if (result is null) return Results.NotFound();

                return Results.Ok(result);
            });
        }
    }
}
