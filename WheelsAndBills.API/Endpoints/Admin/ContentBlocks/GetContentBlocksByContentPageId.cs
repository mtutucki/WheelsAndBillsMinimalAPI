using WheelsAndBills.Application.Features.Admin.ContentBlocks;

namespace WheelsAndBills.API.Endpoints.Admin.ContentBlocks
{
    public static class GetContentBlocksByContentPageId
    {
        public static RouteHandlerBuilder MapGetContentBlocksByContentPageId(
            this RouteGroupBuilder app)
        {
            return app.MapGet("/pages/{contentPageId:guid}/blocks", async (
                Guid contentPageId,
                IContentBlocksService contentBlocks,
                CancellationToken cancellationToken) =>
            {
                var blocks = await contentBlocks.GetByContentPageIdAsync(contentPageId, cancellationToken);

                return Results.Ok(blocks);
            });
        }
    }
}
