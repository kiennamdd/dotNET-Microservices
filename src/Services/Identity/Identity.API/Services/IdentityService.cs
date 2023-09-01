
using Identity.API.Domain.Entities;
using Identity.API.Interfaces;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ITokenService _tokenService;

        public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        public async Task<bool> AssignRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if(user is null)
                return false;

            role = role.Trim().ToUpper();
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if(!roleExists)
            {
                await _roleManager.CreateAsync(new ApplicationRole
                {
                    Name = role
                });
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded;
        }

        public async Task<bool> CreateUserAsync(ApplicationUser user, string password)
        {
            if(string.IsNullOrEmpty(password) || user is null)
                return false;

            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }

        public async Task<string> SignInAsync(string email, string password)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return string.Empty;

            var user = await _userManager.Users.Where(o => o.Email == email).FirstOrDefaultAsync();
            if(user is null)
                return string.Empty;

            var isCorrectPassword = await _userManager.CheckPasswordAsync(user, password);
            if(!isCorrectPassword)
                return string.Empty;

            var roles = await _userManager.GetRolesAsync(user);

            string accessToken = _tokenService.GenerateToken(user, roles);
            return accessToken;
        }

        public Task<string> RefreshAccessToken(Guid userId, string refreshToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SignOutAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser?> FindUserByEmailAsync(string email)
        {
            var user = await _userManager.Users.Where(o => o.Email == email).FirstOrDefaultAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _userManager.Users.Where(o => o.Id.Equals(userId)).FirstOrDefaultAsync();
            if(user is null)
                return false;

            var result =  await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}