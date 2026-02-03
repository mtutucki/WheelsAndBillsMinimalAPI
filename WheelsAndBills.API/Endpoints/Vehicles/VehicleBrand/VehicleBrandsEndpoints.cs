using WheelsAndBills.Application.Features.Vehicles.VehicleBrands;

namespace WheelsAndBills.API.Endpoints.Vehicles.VehicleBrand
{
    public static class VehicleBrandsEndpoints
    {
        public static RouteHandlerBuilder MapGetVehicleBrands(this RouteGroupBuilder app)
        {
            return app.MapGet("", async (
                IVehicleBrandsService vehicleBrands,
                CancellationToken cancellationToken) =>
            {
                var brands = await vehicleBrands.GetAllAsync(cancellationToken);

                return Results.Ok(brands);
            });
        }

        public static RouteHandlerBuilder MapGetVehicleBrandById(this RouteGroupBuilder app)
        {
            return app.MapGet("/{id:guid}", async (
                Guid id,
                IVehicleBrandsService vehicleBrands,
                CancellationToken cancellationToken) =>
            {
                var brand = await vehicleBrands.GetByIdAsync(id, cancellationToken);

                return brand is null
                    ? Results.NotFound()
                    : Results.Ok(brand);
            });
        }

        public static RouteHandlerBuilder MapCreateVehicleBrand(this RouteGroupBuilder app)
        {
            return app.MapPost("", async (
                CreateVehicleBrandDTO request,
                IVehicleBrandsService vehicleBrands,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleBrands.CreateAsync(request, cancellationToken);
                if (!result.Success)
                    return Results.BadRequest(result.Error);

                return Results.Created(
                    $"/vehicle-brands/{result.Data!.Id}",
                    result.Data
                );
            });
        }

        public static RouteHandlerBuilder MapUpdateVehicleBrand(this RouteGroupBuilder app)
        {
            return app.MapPut("/{id:guid}", async (
                Guid id,
                UpdateVehicleBrandDTO request,
                IVehicleBrandsService vehicleBrands,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleBrands.UpdateAsync(id, request, cancellationToken);
                if (!result.Success)
                    return result.Error == "NotFound"
                        ? Results.NotFound()
                        : Results.BadRequest(result.Error);

                return Results.Ok(result.Data);
            });
        }

        public static RouteHandlerBuilder MapDeleteVehicleBrand(this RouteGroupBuilder app)
        {
            return app.MapDelete("/{id:guid}", async (
                Guid id,
                IVehicleBrandsService vehicleBrands,
                CancellationToken cancellationToken) =>
            {
                var result = await vehicleBrands.DeleteAsync(id, cancellationToken);
                if (!result.Success)
                    return Results.NotFound();

                return Results.NoContent();
            });
        }
    }
}
