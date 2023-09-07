using Cart.API.Domain.Constants;
using Cart.API.Interfaces.Infrastructure;
using Cart.API.Models;
using Newtonsoft.Json;

namespace Cart.API.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DiscountService> _logger;

        public DiscountService(IHttpClientFactory httpClientFactory, ILogger<DiscountService> logger)
        {
            _httpClientFactory = httpClientFactory;    
            _logger = logger;
        }
        
        public async Task<CouponDto?> GetCouponByCodeAsync(string couponCode)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient(ApiServiceNames.CatalogApi);

                HttpResponseMessage responseMessage = await client.GetAsync($"/api/discount/coupons/withcode/{couponCode}");
        
                string content = await responseMessage.Content.ReadAsStringAsync();
        
                ResponseDto? responseDto = JsonConvert.DeserializeObject<ResponseDto>(content);
                if(responseDto is null || responseDto.IsSuccess == false)
                    return null;
        
                string resultStr = Convert.ToString(responseDto.Result) ?? string.Empty;
                
                CouponDto? productDto = JsonConvert.DeserializeObject<CouponDto>(resultStr);
        
                return productDto;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while calling Discount service: GetCouponByCode");
                return null;
            }
        }

        public double GetFinalValueAfterDiscount(double value, int discountPercent, double discountAmount, double minValue, double maxDiscountAmount)
        {
            if(value < minValue)
                return value;

            double totalDiscount = 0;

            if(discountAmount > 0)
            {
                totalDiscount = discountAmount;
            }
            else if(discountPercent > 0)
            {
                totalDiscount = value * discountPercent / 100;
            }

            double finalValue = (maxDiscountAmount > 0) ? (value - Math.Min(totalDiscount, maxDiscountAmount)) 
                                                        : (value - totalDiscount);
            
            return Math.Max(finalValue, 0);
        }
    }
}