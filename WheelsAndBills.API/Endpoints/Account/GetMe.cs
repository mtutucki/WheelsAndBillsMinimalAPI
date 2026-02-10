using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WheelsAndBills.Application.Features.Account;

namespace WheelsAndBills.API.Endpoints.Account
{
    public static class GetMe
    {
        public static RouteHandlerBuilder MapGetMe(this RouteGroupBuilder app)
        {
            return app.MapGet("/me", [Authorize] async (
                HttpRequest request,
                ClaimsPrincipal user,
                IAccountService accountService,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userIdClaim is null)
                    return Results.Unauthorized();

                var userId = Guid.Parse(userIdClaim);

                var me = await accountService.GetMeAsync(userId, cancellationToken);

                if (me is null)
                    return Results.NotFound();

                if (!string.IsNullOrWhiteSpace(me.AvatarUrl) && me.AvatarUrl.StartsWith("/"))
                {
                    var baseUrl = $"{request.Scheme}://{request.Host}";
                    me = me with { AvatarUrl = $"{baseUrl}{me.AvatarUrl}" };
                }

                return Results.Ok(me);
            });
        }
    }
}
