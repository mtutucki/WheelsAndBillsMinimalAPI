using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Domain.Entities.Admin;

namespace WheelsAndBills.API.Middleware
{
    internal static class ErrorLogState
    {
        public static bool Enabled { get; set; } = true;
    }

    public class ErrorLogMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAppDbContext db)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) when (!IsValidationException(ex))
            {
                if (ErrorLogState.Enabled)
                {
                    try
                    {
                        var userIdRaw = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                        Guid? userId = null;
                        if (!string.IsNullOrWhiteSpace(userIdRaw) && Guid.TryParse(userIdRaw, out var parsed))
                        {
                            userId = parsed;
                        }
                        var log = new ErrorLog
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            Source = "Server",
                            Message = ex.Message,
                            StackTrace = ex.ToString(),
                            Path = context.Request.Path,
                            Method = context.Request.Method,
                            StatusCode = 500,
                            UserAgent = context.Request.Headers.UserAgent.ToString(),
                            CreatedAt = DateTime.UtcNow
                        };

                        db.ErrorLogs.Add(log);
                        await db.SaveChangesAsync();
                    }
                    catch
                    {
                        ErrorLogState.Enabled = false;
                    }
                }

                throw;
            }
        }

        private static bool IsValidationException(Exception ex)
        {
            if (ex is ValidationException) return true;
            var name = ex.GetType().Name;
            return name.Contains("Validation", StringComparison.OrdinalIgnoreCase)
                   || name.Contains("BadHttpRequest", StringComparison.OrdinalIgnoreCase);
        }
    }
}
