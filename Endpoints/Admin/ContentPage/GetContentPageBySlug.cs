using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Endpoints.Admin.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.ContentPages
{
    public static class GetContentPageBySlug
    {
        public static RouteHandlerBuilder MapGetContentPageBySlug(
            this RouteGroupBuilder app)
        {
            return app.MapGet("/pages/slug/{slug}", async (
                string slug,
                AppDbContext db) =>
                {
                    var page = await db.ContentPage
                        .Where(p => p.Slug == slug)
                        .Select(p => new ContentPagePublicDto(
                            p.Id,
                            p.Title,
                            p.Slug,
                            p.Blocks.Select(b => new ContentBlockPublicDto(
                                b.Slot,
                                b.Content
                            ))
                        ))
                        .FirstOrDefaultAsync();

                    return page == null
                        ? Results.NotFound()
                        : Results.Ok(page);
                });
        }
    }
}
