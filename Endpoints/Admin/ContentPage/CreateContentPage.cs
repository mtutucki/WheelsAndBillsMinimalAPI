using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Domain.Entities.Admin;
using WheelsAndBillsAPI.Endpoints.Admin.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.ContentPages
{
    public static class CreateContentPage
    {
        public static RouteHandlerBuilder MapCreateContentPage(this RouteGroupBuilder app)
        {
            return app.MapPost("/pages", async (
                CreateContentPageDTO request,
                AppDbContext db) =>
                {
                    var slugExists = await db.ContentPage
                        .AnyAsync(p => p.Slug == request.Slug);

                    if (slugExists)
                        return Results.BadRequest("Slug already exists");

                    var page = new ContentPage
                    {
                        Id = Guid.NewGuid(),
                        Title = request.Title,
                        Slug = request.Slug
                    };

                    db.ContentPage.Add(page);
                    await db.SaveChangesAsync();

                    return Results.Created(
                        $"/pages/{page.Id}",
                        new GetContentPageDTO(page.Id, page.Title, page.Slug)
                    );
                });
        }
    }
}
