using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Endpoints.Admin.DTO;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Admin.Dictionaries
{
    public static class DictionaryEndpoints
    {
        // GET ALL
        public static RouteHandlerBuilder MapGetDictionaries(this RouteGroupBuilder app)
        {
            return app.MapGet("/dictionaries", async (AppDbContext db) =>
            {
                var dictionaries = await db.Dictionaries
                    .OrderBy(d => d.Code)
                    .Select(d => new GetDictionaryDTO(d.Id, d.Code))
                    .ToListAsync();

                return Results.Ok(dictionaries);
            });
        }

        // GET BY ID
        public static RouteHandlerBuilder MapGetDictionaryById(this RouteGroupBuilder app)
        {
            return app.MapGet("/dictionaries/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var dictionary = await db.Dictionaries
                    .Where(d => d.Id == id)
                    .Select(d => new GetDictionaryDTO(d.Id, d.Code))
                    .FirstOrDefaultAsync();

                return dictionary is null
                    ? Results.NotFound()
                    : Results.Ok(dictionary);
            });
        }

        // CREATE
        public static RouteHandlerBuilder MapCreateDictionary(this RouteGroupBuilder app)
        {
            return app.MapPost("/dictionaries", async (
                CreateDictionaryDTO request,
                AppDbContext db) =>
            {
                var exists = await db.Dictionaries
                    .AnyAsync(d => d.Code == request.Code);

                if (exists)
                    return Results.BadRequest("Dictionary code already exists");

                var dictionary = new Domain.Entities.Admin.Dictionary
                {
                    Id = Guid.NewGuid(),
                    Code = request.Code
                };

                db.Dictionaries.Add(dictionary);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/dictionaries/{dictionary.Id}",
                    new GetDictionaryDTO(dictionary.Id, dictionary.Code)
                );
            });
        }

        // UPDATE (PUT)
        public static RouteHandlerBuilder MapUpdateDictionary(this RouteGroupBuilder app)
        {
            return app.MapPut("/dictionaries/{id:guid}", async (
                Guid id,
                UpdateDictionaryDTO request,
                AppDbContext db) =>
            {
                var dictionary = await db.Dictionaries.FindAsync(id);
                if (dictionary is null)
                    return Results.NotFound();

                var exists = await db.Dictionaries
                    .AnyAsync(d => d.Code == request.Code && d.Id != id);

                if (exists)
                    return Results.BadRequest("Dictionary code already exists");

                dictionary.Code = request.Code;
                await db.SaveChangesAsync();

                return Results.Ok(new GetDictionaryDTO(
                    dictionary.Id,
                    dictionary.Code
                ));
            });
        }

        // DELETE
        public static RouteHandlerBuilder MapDeleteDictionary(this RouteGroupBuilder app)
        {
            return app.MapDelete("/dictionaries/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var dictionary = await db.Dictionaries.FindAsync(id);
                if (dictionary is null)
                    return Results.NotFound();

                db.Dictionaries.Remove(dictionary);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
