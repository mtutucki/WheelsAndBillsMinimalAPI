using Azure.Core;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Domain.Entities.Admin;
using WheelsAndBillsAPI.Endpoints.Admin.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.ContentBlocks
{
    public static class CreateContentBlocks
    {
        public static RouteHandlerBuilder MapCreateContentBlocks(this RouteGroupBuilder app)
        {
            return app.MapPost("/pages/blocks", async (
                PostContentBlockDTO request,
                AppDbContext db) =>
            {
                var block = new ContentBlock
                {
                    Id = Guid.NewGuid(),
                    ContentPageId = request.ContentPageId,
                    Content = request.Content,
                    Slot = request.Slot
                };

                db.ContentBlocks.Add(block );
                await db.SaveChangesAsync();

                return Results.Created($"/pages/blocks/{block.Id}", block.Id);
            });
        }
    }
}
