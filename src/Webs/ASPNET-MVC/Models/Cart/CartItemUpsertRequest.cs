
using System.ComponentModel.DataAnnotations;

namespace ASPNET_MVC.Models.Cart
{
    public class CartItemUpsertRequest
    {
        [Required]
        public Guid ProductId { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}