
namespace Identity.API.Models
{
    public class RegisterRequest
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // TEST ONLY
        public string Role { get; set; } = string.Empty;
    }
}