
namespace Cart.API.Models
{
    public class ShoppingCartDto
    {
        public Guid Id { get; set; }
        public string? AppliedCouponCode { get; set; }
        public double DiscountAmount { get; set; }
        public double DiscountPercent { get; set; }
        public double CartTocal { get; set; }
        public double TotalDiscountValue { get; set; }

        public IEnumerable<CartItemDto> Items { get; set; }

        public ShoppingCartDto()
        {
            Items = new List<CartItemDto>();
        }
    }
}