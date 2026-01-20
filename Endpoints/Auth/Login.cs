using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WheelsAndBills.Domain.Entities.Auth;
using WheelsAndBillsAPI.Endpoints.Auth.DTO;

namespace WheelsAndBillsAPI.Endpoints.Auth
{
    public static class Login
    {
        public static RouteHandlerBuilder MapLogin(this RouteGroupBuilder app)
        {
            return app.MapPost("/login", async (
                LoginDTO request,
                UserManager<ApplicationUser> userManager,
                IConfiguration config) =>
            {
                var user = await userManager.FindByEmailAsync(request.Email);

                if (user == null)
                    return Results.Unauthorized();

                var valid = await userManager.CheckPasswordAsync(user, request.Password);
                if (!valid)
                    return Results.Unauthorized();

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!)
            };

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(config["Jwt:Key"]!));

                var token = new JwtSecurityToken(
                    issuer: config["Jwt:Issuer"],
                    audience: config["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(
                        int.Parse(config["Jwt:ExpiresMinutes"]!)),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

                return Results.Ok(new
                {
                    accessToken = new JwtSecurityTokenHandler().WriteToken(token)
                });
            });
        }
    }
}
