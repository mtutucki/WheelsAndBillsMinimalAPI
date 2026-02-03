using WheelsAndBills.Application.DTOs.Admin.DTO;
using WheelsAndBills.Application.Features.Admin.ContentBlocks;

namespace WheelsAndBills.API.Endpoints.Admin.ContentBlocks
{
    public static class GetContentBlocks
    {
        public static RouteHandlerBuilder MapGetContentBlocks(
            this RouteGroupBuilder app)
        {
            return app.MapGet("/pages/blocks", async (
                IContentBlocksService contentBlocks,
                CancellationToken cancellationToken) =>
            {
                var blocks = await contentBlocks.GetAllAsync(cancellationToken);

                return Results.Ok(blocks);
            });
        }
    }
}
