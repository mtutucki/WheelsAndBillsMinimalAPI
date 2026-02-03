using WheelsAndBills.Application.DTOs.Admin.DTO;
using WheelsAndBills.Application.Features.Admin.ContentPages;

namespace WheelsAndBills.API.Endpoints.Admin.ContentPages
{
    public static class GetContentPageBySlug
    {
        public static RouteHandlerBuilder MapGetContentPageBySlug(this RouteGroupBuilder app)
        {
            return app.MapGet("/pages/slug/{slug}", async (
                string slug,
                IContentPagesService contentPages,
                CancellationToken cancellationToken) =>
            {
                var page = await contentPages.GetBySlugAsync(slug, cancellationToken);

                return page is null
                    ? Results.NotFound()
                    : Results.Ok(page);
            });
        }
    }
}
