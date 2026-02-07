namespace WheelsAndBills.API.Endpoints.Dashboard
{
    public static class DashboardEndpoints
    {
        public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app)
        {
            var dashboard = app
                .MapGroup("/dashboard")
                .WithTags("Dashboard")
                .RequireAuthorization();

            dashboard.MapGetDashboard();

            return app;
        }
    }
}
