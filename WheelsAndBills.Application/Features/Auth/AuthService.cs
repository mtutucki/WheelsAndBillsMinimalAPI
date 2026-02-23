using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WheelsAndBills.Application.Abstractions.Results;
using WheelsAndBills.Application.DTOs.Auth.DTO;
using WheelsAndBills.Domain.Entities.Auth;

namespace WheelsAndBills.Application.Features.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        public async Task<ServiceResult<RegisterResult>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errorDetails = string.Join(
                    " | ",
                    result.Errors.Select(e => $"{e.Code}:{e.Description}")
                );
                return ServiceResult<RegisterResult>.Fail(errorDetails);
            }

            if (await _roleManager.RoleExistsAsync("User"))
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            return ServiceResult<RegisterResult>.Ok(new RegisterResult(user.Id, user.Email!));
        }

        public async Task<ServiceResult<string>> LoginAsync(LoginDTO request, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return ServiceResult<string>.Fail("Unauthorized");

            var valid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!valid)
                return ServiceResult<string>.Fail("Unauthorized");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_config["Jwt:ExpiresMinutes"]!)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return ServiceResult<string>.Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
