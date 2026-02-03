using WheelsAndBills.Application.DTOs.Admin.DTO;
using WheelsAndBills.Application.Features.Admin.ContentPages;

namespace WheelsAndBills.API.Endpoints.Admin.ContentPages
{
    public static class CreateContentPage
    {
        public static RouteHandlerBuilder MapCreateContentPage(this RouteGroupBuilder app)
        {
            return app.MapPost("/pages", async (
                CreateContentPageDTO request,
                IContentPagesService contentPages,
                CancellationToken cancellationToken) =>
            {
                var result = await contentPages.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/pages/{result.Data!.Id}",
                    result.Data
                );
            });
        }
    }
}
