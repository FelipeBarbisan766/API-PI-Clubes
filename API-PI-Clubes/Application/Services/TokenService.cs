using API_PI_Clubes.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_PI_Clubes.Application.Services
{
    public class TokenService
    {
        public string Generate(User user)
        {
            var handler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(Configuration.PrivateKey);

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = GenerateClaims(user),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = credentials
            };

            var token = handler.CreateToken(tokenDescription);

            var strToken = handler.WriteToken(token);

            return strToken;
        }

        private static ClaimsIdentity GenerateClaims(User user)
        {
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.AddClaim(new Claim(ClaimTypes.Name, user.Name));
            foreach (var role in user.Role.ToString().Split(','))
            {
                claims.AddClaim(new Claim(ClaimTypes.Role, role.Trim()));
            }
            return claims;
        }
    }
}
