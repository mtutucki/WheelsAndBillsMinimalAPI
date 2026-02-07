namespace WheelsAndBills.API.Endpoints.Account
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
            account.MapChangePassword();
            account.MapDeleteAccount();

            return app;
        }
    }
}
