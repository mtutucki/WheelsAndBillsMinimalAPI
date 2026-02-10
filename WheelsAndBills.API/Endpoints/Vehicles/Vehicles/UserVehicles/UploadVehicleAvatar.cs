using Microsoft.AspNetCore.Authorization;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Domain.Entities.Admin;

namespace WheelsAndBills.API.Endpoints.Vehicles.Vehicles.UserVehicles
{
    public static class UploadVehicleAvatar
    {
        public static RouteHandlerBuilder MapUploadVehicleAvatar(this RouteGroupBuilder app)
        {
            return app.MapPost("/avatar", [Authorize] async (
                HttpRequest request,
                IAppDbContext db,
                IWebHostEnvironment env,
                CancellationToken cancellationToken) =>
            {
                if (!request.HasFormContentType)
                    return Results.BadRequest("Invalid content type");

                var form = await request.ReadFormAsync(cancellationToken);
                var file = form.Files.GetFile("file");
                if (file is null || file.Length == 0)
                    return Results.BadRequest("File is required");

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                var allowed = new HashSet<string> { ".png", ".jpg", ".jpeg", ".webp", ".gif" };
                if (!allowed.Contains(ext))
                    return Results.BadRequest("Unsupported file type");

                var webRoot = env.WebRootPath;
                if (string.IsNullOrWhiteSpace(webRoot))
                {
                    webRoot = Path.Combine(env.ContentRootPath, "wwwroot");
                    Directory.CreateDirectory(webRoot);
                }

                var uploadsRoot = Path.Combine(webRoot, "uploads", "vehicle-avatars");
                Directory.CreateDirectory(uploadsRoot);

                var fileName = $"{Guid.NewGuid():N}{ext}";
                var filePath = Path.Combine(uploadsRoot, fileName);

                await using (var stream = File.Create(filePath))
                {
                    await file.CopyToAsync(stream, cancellationToken);
                }

                var relativePath = $"/uploads/vehicle-avatars/{fileName}";
                var resource = new FileResource
                {
                    Id = Guid.NewGuid(),
                    FileName = file.FileName,
                    FilePath = relativePath,
                    UploadedAt = DateTime.UtcNow
                };

                db.FileResources.Add(resource);
                await db.SaveChangesAsync(cancellationToken);

                var baseUrl = $"{request.Scheme}://{request.Host}";
                return Results.Ok(new
                {
                    id = resource.Id,
                    fileName = resource.FileName,
                    filePath = resource.FilePath,
                    url = $"{baseUrl}{relativePath}"
                });
            });
        }
    }
}
