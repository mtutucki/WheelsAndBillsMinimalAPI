using WheelsAndBillsAPI.Endpoints.Admin.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.ContentBlocks
{
    public static class UpdateContentBlocks
    {
        public static RouteHandlerBuilder MapUpdateContentBlocks(this RouteGroupBuilder app)
        {
            return app.MapPut("/pages/blocks/{id}", async (
                Guid id,
                PostContentBlockDTO request,
                AppDbContext db) =>
            {
                var block = await db.ContentBlocks.FindAsync(id);
                if (block == null)
                    return Results.NotFound();


                block.ContentPageId = request.ContentPageId; 
                block.Slot = request.Slot;
                block.Content = request.Content;

                await db.SaveChangesAsync();

                return Results.Ok(block);
            });
        }
    }
}
