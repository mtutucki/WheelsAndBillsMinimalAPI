using System.Security.Claims;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Domain.Entities.Admin;

namespace WheelsAndBills.API.Middleware
{
    internal static class AuditLogState
    {
        public static bool Enabled { get; set; } = true;
    }

    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAppDbContext db)
        {
            await _next(context);

            try
            {
                if (!AuditLogState.Enabled)
                    return;

                var method = context.Request.Method;
                if (HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method))
                    return;

                var userIdRaw = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Guid? userId = null;
                if (!string.IsNullOrWhiteSpace(userIdRaw) && Guid.TryParse(userIdRaw, out var parsed))
                {
                    userId = parsed;
                }

                var log = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Method = method,
                    Path = context.Request.Path,
                    StatusCode = context.Response.StatusCode,
                    CreatedAt = DateTime.UtcNow
                };

                db.AuditLogs.Add(log);
                await db.SaveChangesAsync();
            }
            catch
            {
                // Ignore logging failures (e.g., missing table) to avoid breaking requests.
                AuditLogState.Enabled = false;
            }
        }
    }
}
