
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Cart.API.Interfaces;

namespace Cart.API.Services
{
    public class CurrentUser: ICurrentUser
    {
        private readonly ClaimsPrincipal? _user;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _user = httpContextAccessor.HttpContext?.User;
        }

        public Guid GetUserId()
        {
            if(_user is null)
                return Guid.Empty;

            string? userId = _user.Claims.FirstOrDefault(o => o.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if(Guid.TryParse(userId, out Guid result))
            {
                return result;
            }

            return Guid.Empty;
        }

        public bool IsInRole(string role)
        {
            if(_user is null)
                return false;

            bool isInRole =  _user.Claims.Any(o => o.Type == ClaimTypes.Role && o.Value == role);
            return isInRole;
        }

        public bool IsMatchCurrentUserId(string userIdToCheck, out Guid userIdResult)
        {
            userIdResult = Guid.Empty;

            if(!Guid.TryParse(userIdToCheck, out Guid guid))
                return false;

            Guid currentUserId = GetUserId();

            if(currentUserId != Guid.Empty && currentUserId != guid)
                return false;

            userIdResult = guid;
            return true;
        }
    }
}