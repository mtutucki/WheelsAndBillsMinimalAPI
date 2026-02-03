using static WheelsAndBills.Application.DTOs.Events.EventsDTO;
using WheelsAndBills.Application.Features.Events.Workshops;

namespace WheelsAndBills.API.Endpoints.Events.Workshops
{
    public static class WorkshopsEndpoints
    {
        public static RouteHandlerBuilder MapGetWorkshops(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IWorkshopsService workshops,
                CancellationToken cancellationToken) =>
            {
                var workshopsList = await workshops.GetAllAsync(cancellationToken);

                return Results.Ok(workshopsList);
            });
        }

        public static RouteHandlerBuilder MapGetWorkshopById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IWorkshopsService workshops,
                CancellationToken cancellationToken) =>
            {
                var workshop = await workshops.GetByIdAsync(id, cancellationToken);

                return workshop is null
                    ? Results.NotFound()
                    : Results.Ok(workshop);
            });
        }

        public static RouteHandlerBuilder MapCreateWorkshop(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateWorkshopDTO request,
                IWorkshopsService workshops,
                CancellationToken cancellationToken) =>
            {
                var result = await workshops.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/workshops/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateWorkshop(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateWorkshopDTO request,
                IWorkshopsService workshops,
                CancellationToken cancellationToken) =>
            {
                var result = await workshops.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteWorkshop(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IWorkshopsService workshops,
                CancellationToken cancellationToken) =>
            {
                var result = await workshops.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
