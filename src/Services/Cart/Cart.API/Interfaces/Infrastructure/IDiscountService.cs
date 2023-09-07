
using Cart.API.Models;

namespace Cart.API.Interfaces.Infrastructure
{
    public interface IDiscountService
    {
        Task<CouponDto?> GetCouponByCodeAsync(string couponCode);
        double GetFinalValueAfterDiscount(double value, int discountPercent, double discountAmount, double minValue, double maxDiscountAmount);
    }
}