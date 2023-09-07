
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cart.API.Domain.Common;

namespace Cart.API.Domain.Entities
{
    public class ShoppingCart: EntityBase<Guid>
    {
        public string? AppliedCouponCode { get; set; }

        [NotMapped]
        public double DiscountAmount { get; set; }
        [NotMapped]
        public double DiscountPercent { get; set; }

        [NotMapped]
        public double CartTocal { get; set; }

        public IEnumerable<CartItem> Items { get; set; }

        public ShoppingCart()
        {
            Items = new List<CartItem>();
        }

        public ShoppingCart(Guid userId)
            : base(userId)
        {
            Items = new List<CartItem>();
        }

        public double GetTotal()
        {
            double total = 0;

            foreach(var item in Items)
            {
                total += item.ProductLastPrice * item.Quantity;
            }

            return total;
        }
    }
}