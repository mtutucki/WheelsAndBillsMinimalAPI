using static WheelsAndBills.Application.DTOs.Reports.ReportDTOs;
using WheelsAndBills.Application.Features.Reports.ReportParameters;

namespace WheelsAndBills.API.Endpoints.Reports.ReportParameters
{
    public static class ReportParametersEndpoints
    {
        public static RouteHandlerBuilder MapGetReportParameters(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IReportParametersService reportParameters,
                CancellationToken cancellationToken) =>
            {
                var items = await reportParameters.GetAllAsync(cancellationToken);

                return Results.Ok(items);
            });
        }

        public static RouteHandlerBuilder MapGetReportParameterById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IReportParametersService reportParameters,
                CancellationToken cancellationToken) =>
            {
                var item = await reportParameters.GetByIdAsync(id, cancellationToken);

                return item is null
                    ? Results.NotFound()
                    : Results.Ok(item);
            });
        }

        public static RouteHandlerBuilder MapCreateReportParameter(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateReportParameterDTO request,
                IReportParametersService reportParameters,
                CancellationToken cancellationToken) =>
            {
                var result = await reportParameters.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/report-parameters/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateReportParameter(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateReportParameterDTO request,
                IReportParametersService reportParameters,
                CancellationToken cancellationToken) =>
            {
                var result = await reportParameters.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteReportParameter(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IReportParametersService reportParameters,
                CancellationToken cancellationToken) =>
            {
                var result = await reportParameters.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
