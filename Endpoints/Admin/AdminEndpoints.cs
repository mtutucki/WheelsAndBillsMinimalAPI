using WheelsAndBillsAPI.Endpoints.Admin.Pages;
using WheelsAndBillsAPI.Endpoints.Vehicles;

namespace WheelsAndBillsAPI.Endpoints.Admin
{
    public static class AdminEndpoints
    {
        public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
        {
            var admin = app
                .MapGroup("/admin")
                .WithTags("Admin");

            admin.MapGetPageBySlug();
            admin.MapCreatePage();

            admin.MapGetContentBlocksByContentPageId();
            return app;
        }
    }
}
