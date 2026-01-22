using WheelsAndBillsAPI.Endpoints.Admin.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.ContentBlocks
{
    public static class DeleteContentBlocks
    {
        public static RouteHandlerBuilder MapDeleteContentBlocks(this RouteGroupBuilder app)
        {
            return app.MapDelete("/pages/blocks/{id}", async (
                Guid id,
                AppDbContext db) =>
            {
                var block = await db.ContentBlocks.FindAsync(id);
                if (block == null)
                    return Results.NotFound();


                db.ContentBlocks.Remove(block);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
