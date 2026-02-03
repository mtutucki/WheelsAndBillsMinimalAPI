namespace WheelsAndBills.API.Endpoints.Auth
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var auth = app
                .MapGroup("/auth")
                .WithTags("Authorization");

            auth.MapLogin();
            auth.MapRegister();

            return app;
        }
    }
}
