using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Account
{
    public static class GetMe
    {
        public static IEndpointRouteBuilder MapGetMe(this IEndpointRouteBuilder app)
        {
            app.MapGet("account/me", [Authorize] async (
                AppDbContext db,
                ClaimsPrincipal user) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userIdClaim is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdClaim);

                var me = await db.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new GetMeResponse(
                            u.Id,
                            u.FirstName,
                            u.LastName,
                            u.Email,
                            u.CreatedAt
                        ))
                    .FirstOrDefaultAsync();

                if (me is null)
                    return Results.NotFound();

                return Results.Ok(me);
            });

            return app;
        }

        public record GetMeResponse(
            Guid Id,
            string Name,
            string LastName,
            string Email,
            DateTime CreatedAt);
    }
}
