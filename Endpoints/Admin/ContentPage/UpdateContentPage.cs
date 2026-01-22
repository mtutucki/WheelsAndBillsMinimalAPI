using WheelsAndBillsAPI.Endpoints.Admin.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.ContentPages
{
    public static class UpdateContentPage
    {
        public static RouteHandlerBuilder MapUpdateContentPage(this RouteGroupBuilder app)
        {
            return app.MapPut("/pages/{id}", async (
                Guid id,
                UpdateContentPageDTO request,
                AppDbContext db) =>
            {
                var page = await db.ContentPage.FindAsync(id);
                if (page == null)
                    return Results.NotFound();

                page.Title = request.Title;
                page.Slug = request.Slug;

                await db.SaveChangesAsync();

                return Results.Ok(new GetContentPageDTO(
                    page.Id,
                    page.Title,
                    page.Slug
                ));
            });
        }
    }
}
