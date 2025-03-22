using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TodosMvc.Models.ViewModels;
using TodosMvc.Services.Interfaces;

namespace TodosMvc.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GenerateJwtToken(LoginVm model)
        {
            var jwtConfig = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtConfig["Key"]);
            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, model.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
               issuer: _configuration["Issuer"],
               audience: _configuration["Audience"],
               claims: claims,
               expires: DateTime.UtcNow.AddHours(1),
               signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

    }
}
