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

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
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
                return ServiceResult<RegisterResult>.Fail("Registration failed");

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
