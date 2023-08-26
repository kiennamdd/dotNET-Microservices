
using Catalog.API.Models;

namespace Catalog.API.Interfaces.Infrastructure
{
    public interface IDiscountService
    {
        Task<CouponDto?> GetCouponByCodeAsync(string couponCode);
    }
}