using WheelsAndBills.Application.DTOs.Admin.DTO;
using WheelsAndBills.Application.Features.Admin.ContentBlocks;

namespace WheelsAndBills.API.Endpoints.Admin.ContentBlocks
{
    public static class CreateContentBlocks
    {
        public static RouteHandlerBuilder MapCreateContentBlocks(this RouteGroupBuilder app)
        {
            return app.MapPost("/pages/blocks", async (
                PostContentBlockDTO request,
                IContentBlocksService contentBlocks,
                CancellationToken cancellationToken) =>
            {
                var result = await contentBlocks.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created($"/pages/blocks/{result.Data}", result.Data);
            });
        }
    }
}
