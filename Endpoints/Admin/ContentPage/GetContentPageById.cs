using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Endpoints.Admin.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.ContentPages
{
    public static class GetContentPageById
    {
        public static RouteHandlerBuilder MapGetContentPageById(
            this RouteGroupBuilder app)
        {
            return app.MapGet("/pages/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
                {
                    var page = await db.ContentPage
                        .Where(p => p.Id == id)
                        .Select(p => new GetContentPageDTO(
                            p.Id,
                            p.Title,
                            p.Slug
                        ))
                        .FirstOrDefaultAsync();

                    return page == null
                        ? Results.NotFound()
                        : Results.Ok(page);
                });
        }
    }
}
