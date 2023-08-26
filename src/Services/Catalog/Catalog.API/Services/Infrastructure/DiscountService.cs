using Catalog.API.Domain.Constants;
using Catalog.API.Interfaces.Infrastructure;
using Catalog.API.Models;
using Newtonsoft.Json;

namespace Catalog.API.Services.Infrastructure
{
    public class DiscountService : IDiscountService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DiscountService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CouponDto?> GetCouponByCodeAsync(string code)
        {
            HttpClient client = _httpClientFactory.CreateClient(ApiServices.DiscountApi);

            HttpResponseMessage responseMessage = await client.GetAsync($"/api/discount/coupons/withcode/{code}");

            string content = await responseMessage.Content.ReadAsStringAsync();

            ResponseDto? responseDto = JsonConvert.DeserializeObject<ResponseDto>(content);

            if(responseDto is null || responseDto.IsSuccess == false)
                return null;

            string resultStr = Convert.ToString(responseDto.Result) ?? "";

            CouponDto? couponDto = JsonConvert.DeserializeObject<CouponDto>(resultStr);
            return couponDto;
        }
    }
}