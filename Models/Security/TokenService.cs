using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyFinances.WebApi.Models.Security
{
    public class TokenService
    {
        private IConfiguration Configuration { get; }

        public TokenService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string Generate(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim("email", user.Email),
                new Claim("role", user.IsAdmin ? "Admin" : "User")
            };

            var token = new JwtSecurityToken(Configuration["Jwt:Issuer"],
              Configuration["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMonths(1),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
