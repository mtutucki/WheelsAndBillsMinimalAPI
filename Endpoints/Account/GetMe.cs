using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Endpoints.Account.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Account
{
    public static class GetMe
    {
        public static RouteHandlerBuilder MapGetMe(this RouteGroupBuilder app)
        {
            return app.MapGet("/me", [Authorize] async (
                AppDbContext db,
                ClaimsPrincipal user) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userIdClaim is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdClaim);

                var me = await db.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new GetMeDTO(
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
        }
    }
}
