using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WheelsAndBills.Application.Abstractions.Persistence;
using WheelsAndBills.Application.DTOs.Admin.DTO;

namespace WheelsAndBills.API.Endpoints.Account
{
    public static class GetExperienceLevels
    {
        public static RouteHandlerBuilder MapGetExperienceLevels(this RouteGroupBuilder app)
        {
            return app.MapGet("/experience-levels", [Authorize] async (
                IAppDbContext db,
                CancellationToken cancellationToken) =>
            {
                var dictionaryId = await db.Dictionaries
                    .Where(d => d.Code == "EXPERIENCE_LEVEL")
                    .Select(d => d.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (dictionaryId == Guid.Empty)
                    return Results.Ok(Array.Empty<GetDictionaryItemDTO>());

                var items = await db.DictionaryItems
                    .Where(i => i.DictionaryId == dictionaryId)
                    .OrderBy(i => i.Value)
                    .Select(i => new GetDictionaryItemDTO(i.Id, i.DictionaryId, i.Value))
                    .ToListAsync(cancellationToken);

                return Results.Ok(items);
            });
        }
    }
}
