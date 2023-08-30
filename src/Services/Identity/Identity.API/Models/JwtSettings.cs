
namespace Identity.API.Models
{
    public class JwtSettings
    {
        public string ValidAudience { get; set; } = string.Empty;
        public string ValidIssuer { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public int TokenExpiresInMinutes { get; set; }
        public int RefreshTokenExpiresInDays { get; set; }
    }
}