using ASPNET_MVC.Models.Discount;
using Cart.API.Models;

namespace ASPNET_MVC.Interfaces
{
    public interface IDiscountService: IBaseApiService
    {
        Task<ResponseDto<IEnumerable<CouponDto>>> GetCouponList();
        Task<ResponseDto<CouponDto>> GetCouponById(string couponId);
        Task<ResponseDto<CouponDto>> GetCouponByCode(string couponCode);
        Task<ResponseDto> CreateCoupon(CouponCreateRequest couponCreateRequest);
        Task<ResponseDto> DeleteCoupon(string couponId);
    }
}