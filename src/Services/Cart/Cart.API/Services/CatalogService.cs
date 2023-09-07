using Cart.API.Domain.Constants;
using Cart.API.Interfaces.Infrastructure;
using Cart.API.Models;
using Newtonsoft.Json;

namespace Cart.API.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DiscountService> _logger;

        public CatalogService(IHttpClientFactory httpClientFactory, ILogger<DiscountService> logger)
        {
            _httpClientFactory = httpClientFactory;    
            _logger = logger;
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid productId)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient(ApiServiceNames.CatalogApi);

                HttpResponseMessage responseMessage = await client.GetAsync($"/api/product/details/{productId}");

                string content = await responseMessage.Content.ReadAsStringAsync();

                ResponseDto? responseDto = JsonConvert.DeserializeObject<ResponseDto>(content);
                if(responseDto is null || responseDto.IsSuccess == false)
                    return null;

                string resultStr = Convert.ToString(responseDto.Result) ?? string.Empty;
                
                ProductDto? productDto = JsonConvert.DeserializeObject<ProductDto>(resultStr);

                return productDto;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred while calling Catalog service: GetProductById");

                return null;
            }
        }
    }
}