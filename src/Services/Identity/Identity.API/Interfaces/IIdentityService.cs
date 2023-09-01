
using Identity.API.Domain.Entities;
using Identity.API.Models;

namespace Identity.API.Interfaces
{
    public interface IIdentityService
    {
        Task<bool> CreateUserAsync(ApplicationUser user, string password);
        Task<string> SignInAsync(string email, string password);
        Task<bool> SignOutAsync(Guid userId);
        Task<ApplicationUser?> FindUserByEmailAsync(string email);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<string> RefreshAccessToken(Guid userId, string refreshToken);
        Task<bool> AssignRoleAsync(Guid userId, string role); 
    }
}