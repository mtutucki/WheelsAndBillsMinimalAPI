using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WheelsAndBills.Application.Features.Account;

namespace WheelsAndBills.API.Endpoints.Account
{
    public static class DeleteAccount
    {
        public static RouteHandlerBuilder MapDeleteAccount(this RouteGroupBuilder app)
        {
            return app.MapDelete("/me", async (
                [FromBody] DeleteAccountDTO request,
                ClaimsPrincipal user,
                IAccountService accountService,
                CancellationToken cancellationToken) =>
            {
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();

                var result = await accountService.DeleteAccountAsync(userId, request, cancellationToken);
                if (!result.Success)
                    return result.Error switch
                    {
                        "NotFound" => Results.NotFound(),
                        "InvalidPassword" => Results.BadRequest("Invalid password"),
                        _ => Results.BadRequest(result.Error)
                    };

                return Results.NoContent();
            });
        }
    }
}
