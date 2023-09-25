using Order.Domain.Common;

namespace Order.Domain.Events
{
    public class PaidOrderHasBeenCancelledDomainEvent: DomainEvent
    {
        public Guid OrderId { get; private set; }
        public double OrderTotal { get; private set; }
        public string StripePaymentIntentId { get; private set; }
        public string StripeSessionId { get; private set; }

        public PaidOrderHasBeenCancelledDomainEvent(Guid orderId, double orderTotal, string stripePaymentIntentId, string stripeSessionId)
        {
            OrderId = orderId;
            OrderTotal = orderTotal;
            StripePaymentIntentId = stripePaymentIntentId;
            StripeSessionId = stripeSessionId;
        }
    }
}