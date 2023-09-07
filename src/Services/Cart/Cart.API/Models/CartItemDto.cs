
namespace Cart.API.Models
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public Guid ShoppingCartId { get; set; }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public double ProductOriginalPrice { get; set; }
        public double ProductLastPrice { get; set; }
        public int Quantity { get; set; }
    }
}