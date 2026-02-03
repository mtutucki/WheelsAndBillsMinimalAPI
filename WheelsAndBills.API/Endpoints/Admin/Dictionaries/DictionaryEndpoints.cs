using WheelsAndBills.Application.DTOs.Admin.DTO;
using WheelsAndBills.Application.Features.Admin.Dictionaries;

namespace WheelsAndBills.API.Endpoints.Admin.Dictionaries
{
    public static class DictionaryEndpoints
    {
        // GET ALL
        public static RouteHandlerBuilder MapGetDictionaries(this RouteGroupBuilder app)
        {
            return app.MapGet("/dictionaries", async (
                IDictionariesService dictionaries,
                CancellationToken cancellationToken) =>
            {
                var dictionariesList = await dictionaries.GetAllAsync(cancellationToken);

                return Results.Ok(dictionariesList);
            });
        }

        // GET BY ID
        public static RouteHandlerBuilder MapGetDictionaryById(this RouteGroupBuilder app)
        {
            return app.MapGet("/dictionaries/{id:guid}", async (
                Guid id,
                IDictionariesService dictionaries,
                CancellationToken cancellationToken) =>
            {
                var dictionary = await dictionaries.GetByIdAsync(id, cancellationToken);

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
                IDictionariesService dictionaries,
                CancellationToken cancellationToken) =>
            {
                var result = await dictionaries.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/dictionaries/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        // UPDATE (PUT)
        public static RouteHandlerBuilder MapUpdateDictionary(this RouteGroupBuilder app)
        {
            return app.MapPut("/dictionaries/{id:guid}", async (
                Guid id,
                UpdateDictionaryDTO request,
                IDictionariesService dictionaries,
                CancellationToken cancellationToken) =>
            {
                var result = await dictionaries.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }

        // DELETE
        public static RouteHandlerBuilder MapDeleteDictionary(this RouteGroupBuilder app)
        {
            return app.MapDelete("/dictionaries/{id:guid}", async (
                Guid id,
                IDictionariesService dictionaries,
                CancellationToken cancellationToken) =>
            {
                var result = await dictionaries.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
