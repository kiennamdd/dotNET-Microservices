using ASPNET_MVC.Interfaces;

namespace ASPNET_MVC.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string ACCESS_TOKEN = nameof(ACCESS_TOKEN);

        public TokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void ClearToken()
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(ACCESS_TOKEN);
        }

        public string? GetAccessToken()
        {
            string? accessToken = null;

            _httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue(ACCESS_TOKEN, out accessToken);

            return accessToken;
        }

        public void SetAccessToken(string token)
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(ACCESS_TOKEN, token);
        }
    }
}