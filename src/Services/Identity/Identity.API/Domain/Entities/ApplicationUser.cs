using Microsoft.AspNetCore.Identity;

namespace Identity.API.Domain.Entities
{
    public class ApplicationUser: IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpireTime { get; set; }
    }
}