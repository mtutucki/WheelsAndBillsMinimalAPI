using WheelsAndBills.Application.DTOs.Admin.DTO;
using WheelsAndBills.Application.Features.Admin.ContentBlocks;

namespace WheelsAndBills.API.Endpoints.Admin.ContentBlocks
{
    public static class UpdateContentBlocks
    {
        public static RouteHandlerBuilder MapUpdateContentBlocks(this RouteGroupBuilder app)
        {
            return app.MapPut("/pages/blocks/{id:guid}", async (
                Guid id,
                PostContentBlockDTO request,
                IContentBlocksService contentBlocks,
                CancellationToken cancellationToken) =>
            {
                var result = await contentBlocks.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }
    }
}
