using System.Security.Claims;
using WheelsAndBills.Application.Features.Dashboard;

namespace WheelsAndBills.API.Endpoints.Dashboard
{
    public static class GetCostCompare
    {
        public static RouteHandlerBuilder MapGetCostCompare(this RouteGroupBuilder app)
        {
            return app.MapGet("/costs-compare", async (
                ClaimsPrincipal user,
                string? range,
                IDashboardService dashboardService,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();

                var result = await dashboardService.GetCostCompareAsync(userId, range, cancellationToken);
                return Results.Ok(result);
            });
        }
    }
}
