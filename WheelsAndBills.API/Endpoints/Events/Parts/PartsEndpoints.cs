using static WheelsAndBills.Application.DTOs.Events.EventsDTO;
using WheelsAndBills.Application.Features.Events.Parts;

namespace WheelsAndBills.API.Endpoints.Events.Parts
{
    public static class PartsEndpoints
    {
        public static RouteHandlerBuilder MapGetParts(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IPartsService parts,
                CancellationToken cancellationToken) =>
            {
                var partsList = await parts.GetAllAsync(cancellationToken);

                return Results.Ok(partsList);
            });
        }

        public static RouteHandlerBuilder MapGetPartById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IPartsService parts,
                CancellationToken cancellationToken) =>
            {
                var part = await parts.GetByIdAsync(id, cancellationToken);

                return part is null
                    ? Results.NotFound()
                    : Results.Ok(part);
            });
        }

        public static RouteHandlerBuilder MapCreatePart(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreatePartDTO request,
                IPartsService parts,
                CancellationToken cancellationToken) =>
            {
                var result = await parts.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/parts/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdatePart(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdatePartDTO request,
                IPartsService parts,
                CancellationToken cancellationToken) =>
            {
                var result = await parts.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeletePart(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IPartsService parts,
                CancellationToken cancellationToken) =>
            {
                var result = await parts.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
