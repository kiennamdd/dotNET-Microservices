
using Order.Domain.Entities;

namespace Order.Application.Common.Models
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public Guid ShoppingCartId { get; set; }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public double ProductOriginalPrice { get; set; }
        public double ProductLastPrice { get; set; }
        public string ProductThumbnailUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}