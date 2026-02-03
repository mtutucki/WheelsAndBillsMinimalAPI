using WheelsAndBills.Application.DTOs.Admin.DTO;
using WheelsAndBills.Application.Features.Admin.ContentPages;

namespace WheelsAndBills.API.Endpoints.Admin.ContentPages
{
    public static class DeleteContentPage
    {
        public static RouteHandlerBuilder MapDeleteContentPage(this RouteGroupBuilder app)
        {
            return app.MapDelete("/pages/{id:guid}", async (
                Guid id,
                IContentPagesService contentPages,
                CancellationToken cancellationToken) =>
            {
                var result = await contentPages.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
