using WheelsAndBillsAPI.Endpoints.Vehicles;

namespace WheelsAndBillsAPI.Endpoints.Account
{
    public static class AccountEndpoints
    {
        public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
        {
            var account = app
                .MapGroup("/account")
                .WithTags("Account")
                .RequireAuthorization();

            account.MapGetMe();
            account.MapUpdateMe();

            return app;
        }
    }
}
