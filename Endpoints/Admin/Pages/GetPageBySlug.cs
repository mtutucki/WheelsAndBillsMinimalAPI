using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.Pages
{
    public static class GetPageBySlug
    {
        public static RouteHandlerBuilder MapGetPageBySlug(this RouteGroupBuilder app)
        {
            return app.MapGet("/pages/{slug}", async (
                string slug,
                AppDbContext db) =>
            {
                var page = await db.ContentPage
                    .Include(p => p.Blocks)
                    .FirstOrDefaultAsync(p => p.Slug == slug);

                if (page == null)
                    return Results.NotFound();

                return Results.Ok(new
                {
                    page.Id,
                    page.Slug,
                    page.Title,
                    blocks = page.Blocks
                        .OrderBy(b => b.Id)
                        .Select(b => new
                        {
                            b.Id,
                            b.Slot,
                            b.Content
                        })
                });
            });
        }
    }
}
