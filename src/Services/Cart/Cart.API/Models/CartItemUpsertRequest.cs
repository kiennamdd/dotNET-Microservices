
namespace Cart.API.Models
{
    public class CartItemUpsertRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}