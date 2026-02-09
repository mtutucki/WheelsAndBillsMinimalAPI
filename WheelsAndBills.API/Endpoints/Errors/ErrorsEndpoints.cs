using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Domain.Entities.Admin;

namespace WheelsAndBills.API.Endpoints.Errors
{
    public static class ErrorsEndpoints
    {
        public static IEndpointRouteBuilder MapErrorsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/errors").WithTags("Errors");

            group.MapPost("/client", [AllowAnonymous] async (
                ClientErrorRequest request,
                ClaimsPrincipal user,
                HttpContext httpContext,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                var log = new ErrorLog
                {
                    Id = Guid.NewGuid(),
                    UserId = userId is null ? null : Guid.Parse(userId),
                    Source = "Client",
                    Message = request.Message,
                    StackTrace = request.Stack,
                    Path = request.Url,
                    Method = request.Method,
                    StatusCode = request.StatusCode,
                    UserAgent = request.UserAgent ?? httpContext.Request.Headers.UserAgent.ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                db.ErrorLogs.Add(log);
                await db.SaveChangesAsync(cancellationToken);

                return Results.NoContent();
            });

            return app;
        }

        public record ClientErrorRequest(
            string Message,
            string? Stack,
            string? Url,
            string? Method,
            int? StatusCode,
            string? UserAgent
        );
    }
}
