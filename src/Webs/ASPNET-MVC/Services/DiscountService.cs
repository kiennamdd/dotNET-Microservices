using ASPNET_MVC.Constants;
using ASPNET_MVC.Interfaces;
using ASPNET_MVC.Models;
using ASPNET_MVC.Models.Discount;
using Cart.API.Models;

namespace ASPNET_MVC.Services
{
    public class DiscountService : BaseApiService, IDiscountService
    {
        public DiscountService(IHttpService httpService) : base(httpService)
        {
            Client = httpService.GetClient(ApiServiceNames.DiscountApi);
        }

        public async Task<ResponseDto> CreateCoupon(CouponCreateRequest couponCreateRequest)
        {
            var request = new RequestDto
            {
                HttpMethod = HttpMethod.Post,
                Data = couponCreateRequest,
                Url = "/api/Discount/coupons",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response;
        }

        public async Task<ResponseDto> DeleteCoupon(string couponId)
        {
            var request = new RequestDto
            {
                HttpMethod = HttpMethod.Delete,
                Data = null,
                Url = $"/api/Discount/coupons/{couponId}",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response;
        }

        public async Task<ResponseDto<CouponDto>> GetCouponByCode(string couponCode)
        {
            var request = new RequestDto
            {
                HttpMethod = HttpMethod.Get,
                Data = null,
                Url = $"/api/Discount/coupons/withcode/{couponCode}",
                IncludeAccessToken = false
            };

            ResponseDto response = await SendAsync(request);
            return response.ToResponseDtoWithCastedResult<CouponDto>();
        }

        public async Task<ResponseDto<CouponDto>> GetCouponById(string couponId)
        {
            var request = new RequestDto
            {
                HttpMethod = HttpMethod.Get,
                Data = null,
                Url = $"/api/Discount/coupons/{couponId}",
                IncludeAccessToken = false
            };

            ResponseDto response = await SendAsync(request);
            return response.ToResponseDtoWithCastedResult<CouponDto>();
        }

        public async Task<ResponseDto<IEnumerable<CouponDto>>> GetCouponList()
        {
            var request = new RequestDto
            {
                HttpMethod = HttpMethod.Get,
                Data = null,
                Url = $"/api/Discount/coupons",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response.ToResponseDtoWithCastedResult<IEnumerable<CouponDto>>();
        }
    }
}