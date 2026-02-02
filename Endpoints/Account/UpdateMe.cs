using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Endpoints.Account.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Account
{
    public static class UpdateMe
    {
        public static RouteHandlerBuilder MapUpdateMe(this RouteGroupBuilder app)
        {
            return app.MapPut("/me", [Authorize] async (
                UpdateMyProfileDTO request,
                AppDbContext db,
                ClaimsPrincipal user) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();

                var entity = await db.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (entity is null)
                    return Results.NotFound();

                if (string.IsNullOrWhiteSpace(request.FirstName) ||
                    string.IsNullOrWhiteSpace(request.LastName))
                {
                    return Results.BadRequest("Imię i nazwisko są wymagane");
                }

                entity.FirstName = request.FirstName.Trim();
                entity.LastName = request.LastName.Trim();

                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
