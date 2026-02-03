using WheelsAndBills.Application.DTOs.Admin.DTO;
using WheelsAndBills.Application.Features.Admin.DictionaryItems;

namespace WheelsAndBills.API.Endpoints.Admin.DicionaryItems
{
    public static class DictionaryItemsEndpoints
    {

        public static RouteHandlerBuilder MapGetDictionaryItems(this RouteGroupBuilder app)
        {
            return app.MapGet("/dictionary-items", async (
                IDictionaryItemsService dictionaryItems,
                CancellationToken cancellationToken) =>
            {
                var items = await dictionaryItems.GetAllAsync(cancellationToken);
                
                return Results.Ok(items);
            });
        }

        public static RouteHandlerBuilder MapGetDictionaryItemsByDictionaryId(this RouteGroupBuilder app)
        {
            return app.MapGet("/dictionaries/{dictionaryId:guid}/items", async (
                Guid dictionaryId,
                IDictionaryItemsService dictionaryItems,
                CancellationToken cancellationToken) =>
            {
                var items = await dictionaryItems.GetByDictionaryIdAsync(dictionaryId, cancellationToken);

                return Results.Ok(items);
            });
        }


        public static RouteHandlerBuilder MapCreateDictionaryItem(this RouteGroupBuilder app)
        {
            return app.MapPost("/dictionary-items", async (
                CreateDictionaryItemDTO request,
                IDictionaryItemsService dictionaryItems,
                CancellationToken cancellationToken) =>
            {
                var result = await dictionaryItems.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/dictionary-items/{result.Data!.Id}",
                    result.Data
                );
            });
        }


        public static RouteHandlerBuilder MapUpdateDictionaryItem(this RouteGroupBuilder app)
        {
            return app.MapPut("/dictionary-items/{id:guid}", async (
                Guid id,
                UpdateDictionaryItemDTO request,
                IDictionaryItemsService dictionaryItems,
                CancellationToken cancellationToken) =>
            {
                var result = await dictionaryItems.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }


        public static RouteHandlerBuilder MapDeleteDictionaryItem(this RouteGroupBuilder app)
        {
            return app.MapDelete("/dictionary-items/{id:guid}", async (
                Guid id,
                IDictionaryItemsService dictionaryItems,
                CancellationToken cancellationToken) =>
            {
                var result = await dictionaryItems.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
