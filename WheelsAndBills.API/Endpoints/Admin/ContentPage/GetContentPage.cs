using WheelsAndBills.Application.DTOs.Admin.DTO;
using WheelsAndBills.Application.Features.Admin.ContentPages;

namespace WheelsAndBills.API.Endpoints.Admin.ContentPages
{
    public static class GetContentPage
    {
        public static RouteHandlerBuilder MapGetContentPage(this RouteGroupBuilder app)
        {
            return app.MapGet("/pages", async (
                IContentPagesService contentPages,
                CancellationToken cancellationToken) =>
            {
                var pages = await contentPages.GetAllAsync(cancellationToken);

                return Results.Ok(pages);
            });
        }
    }
}
