using static WheelsAndBillsAPI.Endpoints.Costs.CostsDTO;
using WheelsAndBillsAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WheelsAndBillsAPI.Endpoints.Cost.RecurringCosts
{
    public static class RecurringCostsEndpoints
    {

        public static RouteHandlerBuilder MapGetRecurringCosts(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var costs = await db.RecurringCosts
                    .OrderBy(c => c.IntervalMonths)
                    .Select(c => new GetRecurringCostDTO(
                        c.Id,
                        c.VehicleId,
                        c.CostTypeId,
                        c.Amount,
                        c.IntervalMonths
                    ))
                    .ToListAsync();

                return Results.Ok(costs);
            });
        }

        public static RouteHandlerBuilder MapGetRecurringCostById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var cost = await db.RecurringCosts
                    .Where(c => c.Id == id)
                    .Select(c => new GetRecurringCostDTO(
                        c.Id,
                        c.VehicleId,
                        c.CostTypeId,
                        c.Amount,
                        c.IntervalMonths
                    ))
                    .FirstOrDefaultAsync();

                return cost is null
                    ? Results.NotFound()
                    : Results.Ok(cost);
            });
        }


        public static RouteHandlerBuilder MapCreateRecurringCost(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateRecurringCostDTO request,
                AppDbContext db) =>
            {
                var vehicleExists = await db.Vehicles
                    .AnyAsync(v => v.Id == request.VehicleId);
                if (!vehicleExists)
                    return Results.BadRequest("Vehicle does not exist");

                var costTypeExists = await db.CostTypes
                    .AnyAsync(ct => ct.Id == request.CostTypeId);
                if (!costTypeExists)
                    return Results.BadRequest("CostType does not exist");

                if (request.IntervalMonths <= 0)
                    return Results.BadRequest("IntervalMonths must be greater than 0");

                var cost = new Domain.Entities.Cost.RecurringCost
                {
                    Id = Guid.NewGuid(),
                    VehicleId = request.VehicleId,
                    CostTypeId = request.CostTypeId,
                    Amount = request.Amount,
                    IntervalMonths = request.IntervalMonths
                };

                db.RecurringCosts.Add(cost);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/recurring-costs/{cost.Id}",
                    new GetRecurringCostDTO(
                        cost.Id,
                        cost.VehicleId,
                        cost.CostTypeId,
                        cost.Amount,
                        cost.IntervalMonths
                    )
                );
            });
        }


        public static RouteHandlerBuilder MapUpdateRecurringCost(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateRecurringCostDTO request,
                AppDbContext db) =>
            {
                var cost = await db.RecurringCosts.FindAsync(id);
                if (cost is null)
                    return Results.NotFound();

                var costTypeExists = await db.CostTypes
                    .AnyAsync(ct => ct.Id == request.CostTypeId);
                if (!costTypeExists)
                    return Results.BadRequest("CostType does not exist");

                if (request.IntervalMonths <= 0)
                    return Results.BadRequest("IntervalMonths must be greater than 0");

                cost.CostTypeId = request.CostTypeId;
                cost.Amount = request.Amount;
                cost.IntervalMonths = request.IntervalMonths;

                await db.SaveChangesAsync();

                return Results.Ok(new GetRecurringCostDTO(
                    cost.Id,
                    cost.VehicleId,
                    cost.CostTypeId,
                    cost.Amount,
                    cost.IntervalMonths
                ));
            });
        }


        public static RouteHandlerBuilder MapDeleteRecurringCost(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var cost = await db.RecurringCosts.FindAsync(id);
                if (cost is null)
                    return Results.NotFound();

                db.RecurringCosts.Remove(cost);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
