using Identity.API.Domain.Entities;

namespace Identity.API.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user, IEnumerable<string> roles);
    }
}