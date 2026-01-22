using Microsoft.EntityFrameworkCore;
using WheelsAndBillsAPI.Persistence;

namespace WheelsAndBillsAPI.Endpoints.Vehicles.VehicleBrand
{
    public static class VehicleBrandsEndpoints
    {
        public static RouteHandlerBuilder MapGetVehicleBrands(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (AppDbContext db) =>
            {
                var brands = await db.VehicleBrands
                    .OrderBy(b => b.Name)
                    .Select(b => new GetVehicleBrandDTO(b.Id, b.Name))
                    .ToListAsync();

                return Results.Ok(brands);
            });
        }

        public static RouteHandlerBuilder MapGetVehicleBrandById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var brand = await db.VehicleBrands
                    .Where(b => b.Id == id)
                    .Select(b => new GetVehicleBrandDTO(b.Id, b.Name))
                    .FirstOrDefaultAsync();

                return brand is null
                    ? Results.NotFound()
                    : Results.Ok(brand);
            });
        }

        public static RouteHandlerBuilder MapCreateVehicleBrand(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateVehicleBrandDTO request,
                AppDbContext db) =>
            {
                var exists = await db.VehicleBrands.AnyAsync(b => b.Name == request.Name);
                if (exists)
                    return Results.BadRequest("VehicleBrand already exists");

                var brand = new Domain.Entities.Vehicles.VehicleBrand
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name
                };

                db.VehicleBrands.Add(brand);
                await db.SaveChangesAsync();

                return Results.Created(
                    $"/vehicle-brands/{brand.Id}",
                    new GetVehicleBrandDTO(brand.Id, brand.Name)
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateVehicleBrand(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateVehicleBrandDTO request,
                AppDbContext db) =>
            {
                var brand = await db.VehicleBrands.FindAsync(id);
                if (brand is null)
                    return Results.NotFound();

                var exists = await db.VehicleBrands
                    .AnyAsync(b => b.Name == request.Name && b.Id != id);
                if (exists)
                    return Results.BadRequest("VehicleBrand already exists");

                brand.Name = request.Name;
                await db.SaveChangesAsync();

                return Results.Ok(new GetVehicleBrandDTO(brand.Id, brand.Name));
            });
        }

        public static RouteHandlerBuilder MapDeleteVehicleBrand(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                AppDbContext db) =>
            {
                var brand = await db.VehicleBrands.FindAsync(id);
                if (brand is null)
                    return Results.NotFound();

                db.VehicleBrands.Remove(brand);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
