using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Endpoints.Admin.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.ContentPages
{
    public static class GetContentPage
    {
        public static RouteHandlerBuilder MapGetContentPage(
            this RouteGroupBuilder app)
        {
            return app.MapGet("/pages", async (AppDbContext db) =>
            {
                var pages = await db.ContentPage
                    .OrderBy(p => p.Title)
                    .Select(p => new GetContentPageDTO(
                        p.Id,
                        p.Title,
                        p.Slug
                    ))
                    .ToListAsync();

                return Results.Ok(pages);
            });
        }
    }
}
