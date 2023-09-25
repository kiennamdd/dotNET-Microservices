using Order.Application.Common.Interfaces.Infrastructure;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Stripe;
using Stripe.Checkout;

namespace Order.Infrastructure.Services
{
    public class StripeService : IStripeService
    {
        public StripeService()
        {
             
        }

        public RefundCreateOptions GenerateRefundCreateOptionsForOrder(string stripePaymentIntentId, string reason)
        {
            var options = new RefundCreateOptions
            {
                Reason = reason,
                PaymentIntent = stripePaymentIntentId
            };

            return options;
        }

        public SessionCreateOptions GenerateSessionCreateOptionsForOrder(IEnumerable<OrderItem> items, string successUrl, string cancelUrl, string? couponCode)
        {
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Currency = Currency.USD.ToString(),
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };

            foreach(OrderItem item in items)
            {
                var productData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = item.ProductName,
                    Images = new List<string>{ item.ProductThumbnailUrl }
                };

                var sessionItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.ProductLastPrice * 100),
                        Currency = Currency.USD.ToString(),
                        ProductData = productData
                    },
                    Quantity = item.Quantity
                };

                options.LineItems.Add(sessionItem);
            }

            if(!string.IsNullOrEmpty(couponCode))
            {
                options.Discounts = new List<SessionDiscountOptions>();

                var sessionDiscount = new SessionDiscountOptions
                {
                    Coupon = couponCode
                };

                options.Discounts.Add(sessionDiscount);
            }

            return options;
        }

        public async Task<Session> CreateSessionAsync(SessionCreateOptions options)
        {
            var sessionService = new SessionService();
            Session session = await sessionService.CreateAsync(options);
            return session;
        }
        
        public async Task<bool> ExpireSessionAsync(string sessionId)
        {
            var sessionService = new SessionService();
            await sessionService.ExpireAsync(sessionId);
            return true;
        }

        public async Task<Refund> CreateRefundAsync(RefundCreateOptions options)
        {
            var refundService = new RefundService();
            Refund refund = await refundService.CreateAsync(options);
            return refund;
        }

        public async Task<PaymentIntent> GetPaymentIntentByIdAsync(string stripePaymentIntentId)
        {
            var paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent = await paymentIntentService.GetAsync(stripePaymentIntentId);
            return paymentIntent;
        }

        public async Task<Session> GetSessionByIdAsync(string stripeSessionId)
        {
            var sessionService = new SessionService();
            Session session = await sessionService.GetAsync(stripeSessionId);
            return session;
        }

        public async Task<Refund> GetRefundByIdAsync(string stripeRefundId)
        {
            var refundService = new RefundService();
            Refund refund = await refundService.GetAsync(stripeRefundId);
            return refund;
        }
    }
}