using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.Pages
{
    public static class GetContentBlocksByContentPageId
    {
        public static IEndpointRouteBuilder MapGetContentBlocksByContentPageId(
            this IEndpointRouteBuilder app)
        {
            app.MapGet("/pages/{contentPageId:guid}/blocks", async (
                Guid contentPageId,
                AppDbContext db) =>
            {
                var blocks = await db.ContentBlocks
                    .Where(b => b.ContentPageId == contentPageId)
                    .OrderBy(b => b.Slot)
                    .Select(b => new
                    {
                        b.Id,
                        b.ContentPageId,
                        b.Content,
                        b.Slot
                    })
                    .ToListAsync();

                return Results.Ok(blocks);
            });

            return app;
        }
    }
}
