using ASPNET_MVC.Constants;
using ASPNET_MVC.Interfaces;
using ASPNET_MVC.Models;
using ASPNET_MVC.Models.Cart;
using Cart.API.Models;

namespace ASPNET_MVC.Services
{
    public class CartService : BaseApiService, ICartService
    {
        private readonly string _catalogServiceBaseUrl;

        public CartService(IHttpService httpService, IConfiguration configuration)
            : base(httpService)
        {
            Client = httpService.GetClient(ApiServiceNames.CartApi);
            _catalogServiceBaseUrl = configuration.GetValue<string>("ApiServiceBaseUrls:CatalogApi");
        }

        public async Task<ResponseDto> ApplyCouponForCart(string couponCode)
        {
            var request = new RequestDto {
                HttpMethod = HttpMethod.Post,
                Data = new { couponCode = couponCode },
                Url = "/api/cart/ApplyCoupon",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response;
        }

        public async Task<ResponseDto<ShoppingCartDto>> GetCartDetails(string userId)
        {
            var request = new RequestDto {
                HttpMethod = HttpMethod.Get,
                Data = null,
                Url = $"/api/cart/{userId}/Details",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            var castedResponse = response.ToResponseDtoWithCastedResult<ShoppingCartDto>();

            if(castedResponse.Result != null)
            {
                foreach(CartItemDto item in castedResponse.Result.Items)
                {
                    item.ProductThumbnailUrl = _catalogServiceBaseUrl + "/productimages/" + item.ProductThumbnailFileName;
                }
            }

            return castedResponse;
        }

        public async Task<ResponseDto> RemoveCartItem(int cartItemId)
        {
            var request = new RequestDto {
                HttpMethod = HttpMethod.Post,
                Data = null,
                Url = $"/api/cart/RemoveItem/{cartItemId}",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response;
        }

        public async Task<ResponseDto> UpsertCartItem(CartItemUpsertRequest upsertRequest)
        {
            var request = new RequestDto {
                HttpMethod = HttpMethod.Post,
                Data = upsertRequest,
                Url = $"/api/cart/Upsert",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response;
        }
    }
}