using WheelsAndBills.Application.DTOs.Admin.DTO;
using WheelsAndBills.Application.Features.Admin.FileResources;

namespace WheelsAndBills.API.Endpoints.Admin.FileResources
{
    public static class FileResourcesEndpoints
    {

        public static RouteHandlerBuilder MapGetFileResources(this RouteGroupBuilder app)
        {
            return app.MapGet("/files", async (
                IFileResourcesService fileResources,
                CancellationToken cancellationToken) =>
            {
                var files = await fileResources.GetAllAsync(cancellationToken);

                return Results.Ok(files);
            });
        }

        public static RouteHandlerBuilder MapGetFileResourceById(this RouteGroupBuilder app)
        {
            return app.MapGet("/files/{id:guid}", async (
                Guid id,
                IFileResourcesService fileResources,
                CancellationToken cancellationToken) =>
            {
                var file = await fileResources.GetByIdAsync(id, cancellationToken);

                return file is null
                    ? Results.NotFound()
                    : Results.Ok(file);
            });
        }


        public static RouteHandlerBuilder MapCreateFileResource(this RouteGroupBuilder app)
        {
            return app.MapPost("/files", async (
                CreateFileResourceDTO request,
                IFileResourcesService fileResources,
                CancellationToken cancellationToken) =>
            {
                var result = await fileResources.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/files/{result.Data!.Id}",
                    result.Data
                );
            });
        }


        public static RouteHandlerBuilder MapUpdateFileResource(this RouteGroupBuilder app)
        {
            return app.MapPut("/files/{id:guid}", async (
                Guid id,
                UpdateFileResourceDTO request,
                IFileResourcesService fileResources,
                CancellationToken cancellationToken) =>
            {
                var result = await fileResources.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.Ok(result.Data);
            });
        }


        public static RouteHandlerBuilder MapDeleteFileResource(this RouteGroupBuilder app)
        {
            return app.MapDelete("/files/{id:guid}", async (
                Guid id,
                IFileResourcesService fileResources,
                CancellationToken cancellationToken) =>
            {
                var result = await fileResources.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
