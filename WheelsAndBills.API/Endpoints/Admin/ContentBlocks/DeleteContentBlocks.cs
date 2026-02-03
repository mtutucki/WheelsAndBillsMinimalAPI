using WheelsAndBills.Application.DTOs.Admin.DTO;
using WheelsAndBills.Application.Features.Admin.ContentBlocks;

namespace WheelsAndBills.API.Endpoints.Admin.ContentBlocks
{
    public static class DeleteContentBlocks
    {
        public static RouteHandlerBuilder MapDeleteContentBlocks(this RouteGroupBuilder app)
        {
            return app.MapDelete("/pages/blocks/{id:guid}", async (
                Guid id,
                IContentBlocksService contentBlocks,
                CancellationToken cancellationToken) =>
            {
                var result = await contentBlocks.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
