
using System.ComponentModel.DataAnnotations;
using Cart.API.Domain.Common;

namespace Cart.API.Domain.Entities
{
    public class CartItem: EntityBase<int>
    {
        public Guid ShoppingCartId { get; set; }
        public ShoppingCart? ShoppingCart { get; set; }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public double ProductOriginalPrice { get; set; }
        public double ProductLastPrice { get; set; }
        public string ProductThumbnailFileName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string ProductAppliedCouponCode { get; set; } = string.Empty;
    }
}