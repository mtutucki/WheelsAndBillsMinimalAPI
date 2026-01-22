using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Endpoints.Admin.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.FileResources
{
    public static class FileResourcesEndpoints
    {

        public static RouteHandlerBuilder MapGetFileResources(this RouteGroupBuilder app)
        {
            return app.MapGet("/files", async (AppDbContext db) =>
            {
                var files = await db.FileResources
                    .OrderByDescending(f => f.UploadedAt)
                    .Select(f => new GetFileResourceDTO(
                        f.Id,
                        f.FileName,
                        f.FilePath,
                        f.UploadedAt
                    ))
                    .ToListAsync();

                return Results.Ok(files);
            });
        }

        public static RouteHandlerBuilder MapGetFileResourceById(this RouteGroupBuilder app)
        {
            return app.MapGet("/files/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var file = await db.FileResources
                    .Where(f => f.Id == id)
                    .Select(f => new GetFileResourceDTO(
                        f.Id,
                        f.FileName,
                        f.FilePath,
                        f.UploadedAt
                    ))
                    .FirstOrDefaultAsync();

                return file is null
                    ? Results.NotFound()
                    : Results.Ok(file);
            });
        }


        public static RouteHandlerBuilder MapCreateFileResource(this RouteGroupBuilder app)
        {
            return app.MapPost("/files", async (
                CreateFileResourceDTO request,
                AppDbContext db) =>
            {
                var file = new Domain.Entities.Admin.FileResource
                {
                    Id = Guid.NewGuid(),
                    FileName = request.FileName,
                    FilePath = request.FilePath,
                    UploadedAt = DateTime.UtcNow
                };

                db.FileResources.Add(file);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/files/{file.Id}",
                    new GetFileResourceDTO(
                        file.Id,
                        file.FileName,
                        file.FilePath,
                        file.UploadedAt
                    )
                );
            });
        }


        public static RouteHandlerBuilder MapUpdateFileResource(this RouteGroupBuilder app)
        {
            return app.MapPut("/files/{id:guid}", async (
                Guid id,
                UpdateFileResourceDTO request,
                AppDbContext db) =>
            {
                var file = await db.FileResources.FindAsync(id);
                if (file is null)
                    return Results.NotFound();

                file.FileName = request.FileName;
                file.FilePath = request.FilePath;

                await db.SaveChangesAsync();

                return Results.Ok(new GetFileResourceDTO(
                    file.Id,
                    file.FileName,
                    file.FilePath,
                    file.UploadedAt
                ));
            });
        }


        public static RouteHandlerBuilder MapDeleteFileResource(this RouteGroupBuilder app)
        {
            return app.MapDelete("/files/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var file = await db.FileResources.FindAsync(id);
                if (file is null)
                    return Results.NotFound();

                db.FileResources.Remove(file);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
