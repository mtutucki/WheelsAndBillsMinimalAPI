using WheelsAndBills.Application.DTOs.Admin.DTO;
using WheelsAndBills.Application.Features.Admin.ContentPages;

namespace WheelsAndBills.API.Endpoints.Admin.ContentPages
{
    public static class GetContentPageById
    {
        public static RouteHandlerBuilder MapGetContentPageById(this RouteGroupBuilder app)
        {
            return app.MapGet("/pages/{id:guid}", async (
                Guid id,
                IContentPagesService contentPages,
                CancellationToken cancellationToken) =>
            {
                var page = await contentPages.GetByIdAsync(id, cancellationToken);

                return page is null
                    ? Results.NotFound()
                    : Results.Ok(page);
            });
        }
    }
}
