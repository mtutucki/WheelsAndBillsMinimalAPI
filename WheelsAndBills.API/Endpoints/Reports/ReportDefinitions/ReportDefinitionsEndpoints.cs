using static WheelsAndBills.Application.DTOs.Reports.ReportDTOs;
using WheelsAndBills.Application.Features.Reports.ReportDefinitions;

namespace WheelsAndBills.API.Endpoints.Reports.ReportDefinitions
{
    public static class ReportDefinitionsEndpoints
    {
        public static RouteHandlerBuilder MapGetReportDefinitions(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IReportDefinitionsService reportDefinitions,
                CancellationToken cancellationToken) =>
            {
                var defs = await reportDefinitions.GetAllAsync(cancellationToken);

                return Results.Ok(defs);
            });
        }

        public static RouteHandlerBuilder MapGetReportDefinitionById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IReportDefinitionsService reportDefinitions,
                CancellationToken cancellationToken) =>
            {
                var def = await reportDefinitions.GetByIdAsync(id, cancellationToken);

                return def is null
                    ? Results.NotFound()
                    : Results.Ok(def);
            });
        }

        public static RouteHandlerBuilder MapCreateReportDefinition(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateReportDefinitionDTO request,
                IReportDefinitionsService reportDefinitions,
                CancellationToken cancellationToken) =>
            {
                var result = await reportDefinitions.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/report-definitions/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateReportDefinition(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateReportDefinitionDTO request,
                IReportDefinitionsService reportDefinitions,
                CancellationToken cancellationToken) =>
            {
                var result = await reportDefinitions.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteReportDefinition(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IReportDefinitionsService reportDefinitions,
                CancellationToken cancellationToken) =>
            {
                var result = await reportDefinitions.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
