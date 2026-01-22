using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Endpoints.Admin.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.ContentBlocks
{
    public static class GetContentBlocks
    {
        public static RouteHandlerBuilder MapGetContentBlocks(
            this RouteGroupBuilder app)
        {
            return app.MapGet("/pages/blocks", async (
                AppDbContext db) =>
            {
                var blocks = await db.ContentBlocks
                    .OrderBy(b => b.Slot)
                    .Select(b => new GetContentBlockDTO
                    (
                        b.Id,
                        b.ContentPageId,
                        b.Content,
                        b.Slot
                    ))
                    .ToListAsync();

                return Results.Ok(blocks);
            });
        }
    }
}
