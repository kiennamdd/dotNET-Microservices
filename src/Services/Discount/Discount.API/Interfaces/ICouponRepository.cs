using Discount.API.Domain.Entities;

namespace Discount.API.Interfaces
{
    public interface ICouponRepository: IRepositoryBase<Coupon, string>
    {
        Task<Coupon?> GetByCodeAsync(string code);
    }
}
