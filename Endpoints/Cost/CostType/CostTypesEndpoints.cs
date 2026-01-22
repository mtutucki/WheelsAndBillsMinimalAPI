using static WheelsAndBillsAPI.Endpoints.Costs.CostsDTO;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Domain.Entities.Cost;

namespace WheelsAndBillsAPI.Endpoints.Cost.CostType
{
    public static class CostTypesEndpoints
    {

        public static RouteHandlerBuilder MapGetCostTypes(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var types = await db.CostTypes
                    .OrderBy(t => t.Name)
                    .Select(t => new GetCostTypeDTO(
                        t.Id,
                        t.Name
                    ))
                    .ToListAsync();

                return Results.Ok(types);
            });
        }

        public static RouteHandlerBuilder MapGetCostTypeById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var type = await db.CostTypes
                    .Where(t => t.Id == id)
                    .Select(t => new GetCostTypeDTO(
                        t.Id,
                        t.Name
                    ))
                    .FirstOrDefaultAsync();

                return type is null
                    ? Results.NotFound()
                    : Results.Ok(type);
            });
        }


        public static RouteHandlerBuilder MapCreateCostType(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateCostTypeDTO request,
                AppDbContext db) =>
            {
                var exists = await db.CostTypes
                    .AnyAsync(t => t.Name == request.Name);

                if (exists)
                    return Results.BadRequest("Cost type already exists");

                var type = new Domain.Entities.Cost.CostType
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name
                };

                db.CostTypes.Add(type);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/cost-types/{type.Id}",
                    new GetCostTypeDTO(type.Id, type.Name)
                );
            });
        }


        public static RouteHandlerBuilder MapUpdateCostType(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateCostTypeDTO request,
                AppDbContext db) =>
            {
                var type = await db.CostTypes.FindAsync(id);
                if (type is null)
                    return Results.NotFound();

                var exists = await db.CostTypes
                    .AnyAsync(t => t.Name == request.Name && t.Id != id);

                if (exists)
                    return Results.BadRequest("Cost type already exists");

                type.Name = request.Name;
                await db.SaveChangesAsync();

                return Results.Ok(new GetCostTypeDTO(
                    type.Id,
                    type.Name
                ));
            });
        }


        public static RouteHandlerBuilder MapDeleteCostType(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var type = await db.CostTypes.FindAsync(id);
                if (type is null)
                    return Results.NotFound();

                db.CostTypes.Remove(type);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
