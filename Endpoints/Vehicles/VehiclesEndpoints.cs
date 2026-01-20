namespace WheelsAndBillsAPI.Endpoints.Vehicles
{
    public static class VehiclesEndpoints
    {
        public static IEndpointRouteBuilder MapVehiclesEndpoints(this IEndpointRouteBuilder app)
        {
            var vehicles = app
                .MapGroup("/vehicles")
                .WithTags("Vehicles")
                .RequireAuthorization();

            vehicles.MapGetVehicles();
            vehicles.MapGetVehicleById();
            vehicles.MapCreateVehicle();

            return app;
        }
    }
}
