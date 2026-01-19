using WheelsAndBillsAPI.Persistence;
using WheelsAndBillsAPI.Domain.Entities.Admin;
using Microsoft.EntityFrameworkCore;

namespace WheelsAndBillsAPI.Endpoints.Admin.Pages
{
    public static class CreatePage
    {
        public static IEndpointRouteBuilder MapCreatePage(this IEndpointRouteBuilder app)
        {
            app.MapPost("/admin/pages", async (
                CreatePageRequest request,
                AppDbContext db) =>
            {
                string slug = request.Slug;

                if (slug.StartsWith("/"))
                    slug = slug.TrimStart('/');

                var slugExists = await db.ContentPage
                    .AnyAsync(p => p.Slug == slug);

                if (slugExists)
                    return Results.BadRequest("Slug already exists");

                var page = new ContentPage
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Slug = slug
                };

                db.ContentPage.Add(page);
                await db.SaveChangesAsync();

                return Results.Created($"/pages/{page.Slug}", page);
            });

            return app;
        }

        public record CreatePageRequest(string Title, string Slug);
    }
}
