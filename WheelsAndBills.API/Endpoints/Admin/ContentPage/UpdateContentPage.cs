using WheelsAndBills.Application.DTOs.Admin.DTO;
using WheelsAndBills.Application.Features.Admin.ContentPages;

namespace WheelsAndBills.API.Endpoints.Admin.ContentPages
{
    public static class UpdateContentPage
    {
        public static RouteHandlerBuilder MapUpdateContentPage(this RouteGroupBuilder app)
        {
            return app.MapPut("/pages/{id:guid}", async (
                Guid id,
                UpdateContentPageDTO request,
                IContentPagesService contentPages,
                CancellationToken cancellationToken) =>
            {
                var result = await contentPages.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }
    }
}
