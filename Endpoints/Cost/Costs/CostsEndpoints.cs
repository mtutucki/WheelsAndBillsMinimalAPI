using static WheelsAndBillsAPI.Endpoints.Costs.CostsDTO;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Domain.Entities.Cost;

namespace WheelsAndBillsAPI.Endpoints.Cost.Costs
{
    public static class CostsEndpoints
    {

        public static RouteHandlerBuilder MapGetCosts(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var costs = await db.Costs
                    .OrderByDescending(c => c.Amount)
                    .Select(c => new GetCostDTO(
                        c.Id,
                        c.VehicleEventId,
                        c.CostTypeId,
                        c.Amount
                    ))
                    .ToListAsync();

                return Results.Ok(costs);
            });
        }

        public static RouteHandlerBuilder MapGetCostById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var cost = await db.Costs
                    .Where(c => c.Id == id)
                    .Select(c => new GetCostDTO(
                        c.Id,
                        c.VehicleEventId,
                        c.CostTypeId,
                        c.Amount
                    ))
                    .FirstOrDefaultAsync();

                return cost is null
                    ? Results.NotFound()
                    : Results.Ok(cost);
            });
        }


        public static RouteHandlerBuilder MapCreateCost(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateCostDTO request,
                AppDbContext db) =>
            {
                var eventExists = await db.VehicleEvents
                    .AnyAsync(e => e.Id == request.VehicleEventId);

                if (!eventExists)
                    return Results.BadRequest("VehicleEvent does not exist");

                var costTypeExists = await db.CostTypes
                    .AnyAsync(ct => ct.Id == request.CostTypeId);

                if (!costTypeExists)
                    return Results.BadRequest("CostType does not exist");

                var cost = new Domain.Entities.Cost.Cost
                {
                    Id = Guid.NewGuid(),
                    VehicleEventId = request.VehicleEventId,
                    CostTypeId = request.CostTypeId,
                    Amount = request.Amount
                };

                db.Costs.Add(cost);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/costs/{cost.Id}",
                    new GetCostDTO(
                        cost.Id,
                        cost.VehicleEventId,
                        cost.CostTypeId,
                        cost.Amount
                    )
                );
            });
        }


        public static RouteHandlerBuilder MapUpdateCost(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateCostDTO request,
                AppDbContext db) =>
            {
                var cost = await db.Costs.FindAsync(id);
                if (cost is null)
                    return Results.NotFound();

                var costTypeExists = await db.CostTypes
                    .AnyAsync(ct => ct.Id == request.CostTypeId);

                if (!costTypeExists)
                    return Results.BadRequest("CostType does not exist");

                cost.CostTypeId = request.CostTypeId;
                cost.Amount = request.Amount;

                await db.SaveChangesAsync();

                return Results.Ok(new GetCostDTO(
                    cost.Id,
                    cost.VehicleEventId,
                    cost.CostTypeId,
                    cost.Amount
                ));
            });
        }


        public static RouteHandlerBuilder MapDeleteCost(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var cost = await db.Costs.FindAsync(id);
                if (cost is null)
                    return Results.NotFound();

                db.Costs.Remove(cost);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
