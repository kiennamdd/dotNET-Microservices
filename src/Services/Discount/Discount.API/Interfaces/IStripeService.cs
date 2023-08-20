using Discount.API.Domain.Entities;

namespace Discount.API.Interfaces
{
    public interface IStripeService
    {
        Task<bool> CreateCouponAsync(Coupon coupon);
        Task<bool> DeleteCouponByCodeAsync(string couponCode);
        Task<Stripe.Coupon?> FindCouponAsync(string couponCode);
    }
}
