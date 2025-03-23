using APIBlazor.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static APIBlazor.Service.UserLoginService;

namespace APIBlazor.Service
{
    public class JwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _lifespan;

        public JwtService(IConfiguration configuration)
        {
            _secretKey = configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Секретный ключ JWT не настроен.");
            _issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Секретный ключ JWT не настроен.");
            _audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Секретный ключ JWT не настроен.");

            var lifespanValue = configuration["Jwt:Lifespan"];
            if (string.IsNullOrEmpty(configuration["Jwt:Lifespan"]))
            {
                _lifespan = 60;
            }
            else if (!int.TryParse(lifespanValue, out _lifespan))
            {
                throw new InvalidOperationException("Некорректное значение для JWT Lifespan.");
            }
        }

        public string GenerateJwtToken(UserDto user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? "Не указано"),
                new Claim("description", user.Description ?? "Не указано"),
                new Claim(ClaimTypes.Role, user.Role ?? "Не указано"),
                new Claim(ClaimTypes.Email, user.Email ?? "Не указано")
            };

            if (user.Logins != null)
            {
                foreach (var login in user.Logins)
                {
                    claims.Add(new Claim("logins", login));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_lifespan),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
