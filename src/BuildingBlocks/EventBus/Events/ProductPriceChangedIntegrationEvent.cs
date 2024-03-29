
using EventBus.Common;

namespace EventBus.Events
{
    public class ProductPriceChangedIntegrationEvent: IntegrationEventBase
    {
        public Guid ProductId { get; set; }
        public double Price { get; set; }
        public string AppliedCouponCode { get; set; } = string.Empty;
        public double DiscountAmount { get;set; } = 0;
        public int DiscountPercent { get;set; } = 0;
    }
}