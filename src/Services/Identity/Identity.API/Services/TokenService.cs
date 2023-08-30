using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.API.Domain.Entities;
using Identity.API.Interfaces;
using Identity.API.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;    
        }

        public string GenerateToken(ApplicationUser user, IEnumerable<string> roles)
        {
            byte[] key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            List<Claim> roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            claims.AddRange(roleClaims);

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Audience = _jwtSettings.ValidAudience,
                Issuer = _jwtSettings.ValidIssuer,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.Now.AddMinutes(_jwtSettings.TokenExpiresInMinutes),
                Subject = new ClaimsIdentity(claims)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            JwtSecurityToken securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);
            return tokenHandler.WriteToken(securityToken);
        }
    }
}