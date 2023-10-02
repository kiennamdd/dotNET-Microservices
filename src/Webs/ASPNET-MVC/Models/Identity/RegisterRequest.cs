
using System.ComponentModel.DataAnnotations;

namespace ASPNET_MVC.Models.Identity
{
    public class RegisterRequest
    {
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        // TEST ONLY
        public string Role { get; set; } = string.Empty;
    }
}