using Order.Domain.Entities;
using Order.Domain.Enums;
using Stripe;
using Stripe.Checkout;

namespace Order.Application.Common.Interfaces.Infrastructure
{
    public interface IStripeService
    {
        RefundCreateOptions GenerateRefundCreateOptionsForOrder(string stripePaymentIntentId, string reason);
        SessionCreateOptions GenerateSessionCreateOptionsForOrder(IEnumerable<OrderItem> items, string successUrl, string cancelUrl, string? couponCode);
        Task<Session> CreateSessionAsync(SessionCreateOptions options);
        Task<Refund> CreateRefundAsync(RefundCreateOptions options);
        Task<bool> ExpireSessionAsync(string sessionId);
        Task<Session> GetSessionByIdAsync(string stripeSessionId);
        Task<Refund> GetRefundByIdAsync(string stripeRefundId);
        Task<PaymentIntent> GetPaymentIntentByIdAsync(string stripePaymentIntentId);
    }
}