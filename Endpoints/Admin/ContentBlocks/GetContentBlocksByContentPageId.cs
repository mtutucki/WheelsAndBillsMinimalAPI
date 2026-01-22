using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.ContentBlocks
{
    public static class GetContentBlocksByContentPageId
    {
        public static RouteHandlerBuilder MapGetContentBlocksByContentPageId(
            this RouteGroupBuilder app)
        {
            return app.MapGet("/pages/{contentPageId:guid}/blocks", async (
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
        }
    }
}
