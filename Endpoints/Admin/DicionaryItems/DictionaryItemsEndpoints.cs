using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Endpoints.Admin.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.DicionaryItems
{
    public static class DictionaryItemsEndpoints
    {

        public static RouteHandlerBuilder MapGetDictionaryItems(this RouteGroupBuilder app)
        {
            return app.MapGet("/dictionary-items", async (AppDbContext db) =>
            {
                var items = await db.DictionaryItems
                    .OrderBy(i => i.Value)
                    .Select(i => new GetDictionaryItemDTO(
                        i.Id,
                        i.DictionaryId,
                        i.Value
                    ))
                    .ToListAsync();
                
                return Results.Ok(items);
            });
        }

        public static RouteHandlerBuilder MapGetDictionaryItemsByDictionaryId(this RouteGroupBuilder app)
        {
            return app.MapGet("/dictionaries/{dictionaryId:guid}/items", async (
                Guid dictionaryId,
                AppDbContext db) =>
            {
                var items = await db.DictionaryItems
                    .Where(i => i.DictionaryId == dictionaryId)
                    .OrderBy(i => i.Value)
                    .Select(i => new GetDictionaryItemDTO(
                        i.Id,
                        i.DictionaryId,
                        i.Value
                    ))
                    .ToListAsync();

                return Results.Ok(items);
            });
        }


        public static RouteHandlerBuilder MapCreateDictionaryItem(this RouteGroupBuilder app)
        {
            return app.MapPost("/dictionary-items", async (
                CreateDictionaryItemDTO request,
                AppDbContext db) =>
            {
                var dictionaryExists = await db.Dictionaries
                    .AnyAsync(d => d.Id == request.DictionaryId);

                if (!dictionaryExists)
                    return Results.BadRequest("Dictionary does not exist");

                var exists = await db.DictionaryItems
                    .AnyAsync(i =>
                        i.DictionaryId == request.DictionaryId &&
                        i.Value == request.Value);

                if (exists)
                    return Results.BadRequest("Dictionary item already exists");

                var item = new Domain.Entities.Admin.DictionaryItem
                {
                    Id = Guid.NewGuid(),
                    DictionaryId = request.DictionaryId,
                    Value = request.Value
                };

                db.DictionaryItems.Add(item);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/dictionary-items/{item.Id}",
                    new GetDictionaryItemDTO(
                        item.Id,
                        item.DictionaryId,
                        item.Value
                    )
                );
            });
        }


        public static RouteHandlerBuilder MapUpdateDictionaryItem(this RouteGroupBuilder app)
        {
            return app.MapPut("/dictionary-items/{id:guid}", async (
                Guid id,
                UpdateDictionaryItemDTO request,
                AppDbContext db) =>
            {
                var item = await db.DictionaryItems.FindAsync(id);
                if (item is null)
                    return Results.NotFound();

                var exists = await db.DictionaryItems
                    .AnyAsync(i =>
                        i.DictionaryId == item.DictionaryId &&
                        i.Value == request.Value &&
                        i.Id != id);

                if (exists)
                    return Results.BadRequest("Dictionary item already exists");

                item.Value = request.Value;
                await db.SaveChangesAsync();

                return Results.Ok(new GetDictionaryItemDTO(
                    item.Id,
                    item.DictionaryId,
                    item.Value
                ));
            });
        }


        public static RouteHandlerBuilder MapDeleteDictionaryItem(this RouteGroupBuilder app)
        {
            return app.MapDelete("/dictionary-items/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var item = await db.DictionaryItems.FindAsync(id);
                if (item is null)
                    return Results.NotFound();

                db.DictionaryItems.Remove(item);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
