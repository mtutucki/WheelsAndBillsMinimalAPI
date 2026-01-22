using Azure.Core;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Domain.Entities.Admin;
using WheelsAndBillsAPI.Endpoints.Admin.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.ContentPages
{
    public static class DeleteContentPage
    {
        public static RouteHandlerBuilder MapDeleteContentPage(this RouteGroupBuilder app)
        {
            return app.MapDelete("/pages/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
                {
                    var page = await db.ContentPage.FindAsync(id);
                    if (page == null)
                        return Results.NotFound();

                    db.ContentPage.Remove(page);
                    await db.SaveChangesAsync();

                    return Results.NoContent();
                });
        }
    }
}
