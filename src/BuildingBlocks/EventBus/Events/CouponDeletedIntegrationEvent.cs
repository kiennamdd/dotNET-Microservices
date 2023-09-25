
using EventBus.Common;

namespace EventBus.Events
{
    public class CouponDeletedIntegrationEvent: IntegrationEventBase
    {
        public string CouponId { get; set; } = string.Empty;
        public string CouponCode { get; set; } = string.Empty;
        public double MinOrderTotal { get; set; }
        public int DiscountPercent { get; set; }
        public double DiscountAmount { get; set; }
        public double MaxDiscountAmount { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}