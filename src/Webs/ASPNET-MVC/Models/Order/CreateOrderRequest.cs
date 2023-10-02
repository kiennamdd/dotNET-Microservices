using System.ComponentModel.DataAnnotations;
using ASPNET_MVC.Models.Cart;

namespace ASPNET_MVC.Models.Order
{
    public class CreateOrderRequest
    {
        [Required]
        public Guid CartId { get; set; }
        
        [Required]
        public string Street { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string State { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        [Required]
        public string ZipCode { get; set; } = string.Empty;
    }
}