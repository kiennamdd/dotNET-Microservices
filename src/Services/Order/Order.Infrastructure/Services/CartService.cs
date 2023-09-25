using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Order.Application.Common.Interfaces;
using Order.Application.Common.Interfaces.Infrastructure;
using Order.Application.Common.Models;
using Order.Domain.Constants;

namespace Order.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly HttpClient _client;
        private readonly ILogger<CartService> _logger;

        public CartService(IHttpClientFactory httpClientFactory, ILogger<CartService> logger, ICurrentUser currentUser)
        {
            _client = httpClientFactory.CreateClient(ApiServiceNames.CartApi);
            _logger = logger;
        }

        public async Task<ShoppingCartDto?> GetCartByUserId(Guid userId)
        {
            try
            {
                HttpResponseMessage responseMessage = await _client.GetAsync($"api/Cart/{userId.ToString()}/Details");

                string content = await responseMessage.Content.ReadAsStringAsync();
                ResponseDto? responseDto = JsonConvert.DeserializeObject<ResponseDto>(content);

                ShoppingCartDto? shoppingCartDto = ResponseDto.DeserializeResult<ShoppingCartDto>(responseDto);
                return shoppingCartDto;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Unknown error happened while retrieving data from Cart service, METHOD: {nameof(GetCartByUserId)}");
                return null;
            }
        }
    }
}