using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.DTOs.Admin.DTO;

namespace WheelsAndBills.API.Endpoints.Account
{
    public static class GetDictionaryItemsByCode
    {
        public static RouteHandlerBuilder MapGetDictionaryItemsByCode(this RouteGroupBuilder app)
        {
            return app.MapGet("/dictionaries/{code}/items", [Authorize] async (
                string code,
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var dictionaryId = await db.Dictionaries
                    .Where(d => d.Code == code)
                    .Select(d => d.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (dictionaryId == Guid.Empty)
                    return Results.Ok(Array.Empty<GetDictionaryItemDTO>());

                var items = await db.DictionaryItems
                    .Where(i => i.DictionaryId == dictionaryId)
                    .OrderBy(i => i.Value)
                    .Select(i => new GetDictionaryItemDTO(i.Id, i.DictionaryId, i.Key, i.Value))
                    .ToListAsync(cancellationToken);

                return Results.Ok(items);
            });
        }
    }
}
