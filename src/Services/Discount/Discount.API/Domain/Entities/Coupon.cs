using Discount.API.Domain.Common;

namespace Discount.API.Domain.Entities
{
    public class Coupon: AuditableEntity<string>
    {
        public string UserId { get; set; } = string.Empty;
        public string CouponCode { get; set; } = string.Empty;
        public double MinOrderTotal { get; set; }
        public int DiscountPercent { get; set; }
        public double DiscountAmount { get; set; }
        public double MaxDiscountAmount { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
