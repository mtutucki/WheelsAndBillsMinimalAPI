using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Vehicles.VehicleModel
{
    public static class VehicleModelsEndpoints
    {
        public static RouteHandlerBuilder MapGetVehicleModels(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var models = await db.VehicleModels
                    .Select(m => new GetVehicleModelDTO(
                        m.Id,
                        m.BrandId,
                        m.Name
                    ))
                    .ToListAsync();

                return Results.Ok(models);
            });
        }

        public static RouteHandlerBuilder MapGetVehicleModelById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var model = await db.VehicleModels
                    .Where(m => m.Id == id)
                    .Select(m => new GetVehicleModelDTO(
                        m.Id,
                        m.BrandId,
                        m.Name
                    ))
                    .FirstOrDefaultAsync();

                return model is null
                    ? Results.NotFound()
                    : Results.Ok(model);
            });
        }

        public static RouteHandlerBuilder MapCreateVehicleModel(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateVehicleModelDTO request,
                AppDbContext db) =>
            {
                var brandExists = await db.VehicleBrands
                    .AnyAsync(b => b.Id == request.BrandId);
                if (!brandExists)
                    return Results.BadRequest("VehicleBrand does not exist");

                var exists = await db.VehicleModels
                    .AnyAsync(m => m.BrandId == request.BrandId && m.Name == request.Name);
                if (exists)
                    return Results.BadRequest("VehicleModel already exists");

                var model = new Domain.Entities.Vehicles.VehicleModel
                {
                    Id = Guid.NewGuid(),
                    BrandId = request.BrandId,
                    Name = request.Name
                };

                db.VehicleModels.Add(model);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/vehicle-models/{model.Id}",
                    new GetVehicleModelDTO(
                        model.Id,
                        model.BrandId,
                        model.Name
                    )
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateVehicleModel(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateVehicleModelDTO request,
                AppDbContext db) =>
            {
                var model = await db.VehicleModels.FindAsync(id);
                if (model is null)
                    return Results.NotFound();

                var brandExists = await db.VehicleBrands
                    .AnyAsync(b => b.Id == request.BrandId);
                if (!brandExists)
                    return Results.BadRequest("VehicleBrand does not exist");

                var exists = await db.VehicleModels
                    .AnyAsync(m =>
                        m.BrandId == request.BrandId &&
                        m.Name == request.Name &&
                        m.Id != id);
                if (exists)
                    return Results.BadRequest("VehicleModel already exists");

                model.BrandId = request.BrandId;
                model.Name = request.Name;

                await db.SaveChangesAsync();

                return Results.Ok(new GetVehicleModelDTO(
                    model.Id,
                    model.BrandId,
                    model.Name
                ));
            });
        }

        public static RouteHandlerBuilder MapDeleteVehicleModel(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var model = await db.VehicleModels.FindAsync(id);
                if (model is null)
                    return Results.NotFound();

                db.VehicleModels.Remove(model);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
