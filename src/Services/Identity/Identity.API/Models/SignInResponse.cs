
namespace Identity.API.Models
{
    public class SignInResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}