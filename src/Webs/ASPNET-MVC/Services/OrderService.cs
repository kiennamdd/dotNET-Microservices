using ASPNET_MVC.Constants;
using ASPNET_MVC.Interfaces;
using ASPNET_MVC.Models;
using ASPNET_MVC.Models.Order;
using Cart.API.Models;

namespace ASPNET_MVC.Services
{
    public class OrderService : BaseApiService, IOrderService
    {
        private readonly string _catalogServiceBaseUrl;

        public OrderService(IHttpService httpService, IConfiguration configuration) : base(httpService)
        {
            Client = httpService.GetClient(ApiServiceNames.OrderApi);
            _catalogServiceBaseUrl = configuration.GetValue<string>("ApiServiceBaseUrls:CatalogApi");
        }

        public async Task<ResponseDto> CancelOrder(string orderId)
        {
            var request = new RequestDto
            {
                HttpMethod = HttpMethod.Post,
                Data = null,
                Url = $"/api/Order/cancel/{orderId}",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response;
        }

        public async Task<ResponseDto> ConfirmCheckout(string orderId)
        {
            var request = new RequestDto
            {
                HttpMethod = HttpMethod.Post,
                Data = null,
                Url = $"/api/Order/confirm/{orderId}",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response;
        }

        public async Task<ResponseDto<CheckoutResponse>> CreateCheckout(CheckoutRequest checkoutRequest)
        {
            var request = new RequestDto
            {
                HttpMethod = HttpMethod.Post,
                Data = checkoutRequest,
                Url = $"/api/Order/checkout",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response.ToResponseDtoWithCastedResult<CheckoutResponse>();
        }

        public async Task<ResponseDto<CreateOrderResponse>> CreateOrder(CreateOrderRequest createOrderRequest)
        {
            var request = new RequestDto
            {
                HttpMethod = HttpMethod.Post,
                Data = createOrderRequest,
                Url = $"/api/Order/create",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response.ToResponseDtoWithCastedResult<CreateOrderResponse>();
        }

        public async Task<ResponseDto<CustomerOrderDto>> GetOrderDetails(string orderId)
        {
            var request = new RequestDto
            {
                HttpMethod = HttpMethod.Get,
                Data = null,
                Url = $"/api/Order/details/{orderId}",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            var castedResponse = response.ToResponseDtoWithCastedResult<CustomerOrderDto>();

            if(castedResponse.Result != null)
            {
                foreach(OrderItemDto item in castedResponse.Result.Items)
                {
                    item.ProductThumbnailUrl = _catalogServiceBaseUrl + "/productimages/" + item.ProductThumbnailFileName;
                }
            }

            return castedResponse;
        }

        public async Task<ResponseDto<IEnumerable<CustomerOrderDto>>> GetOrderHistory()
        {
            var request = new RequestDto
            {
                HttpMethod = HttpMethod.Get,
                Data = null,
                Url = $"/api/Order/history",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response.ToResponseDtoWithCastedResult<IEnumerable<CustomerOrderDto>>();
        }

        public async Task<ResponseDto<IEnumerable<CustomerOrderDto>>> GetOrderList()
        {
            var request = new RequestDto
            {
                HttpMethod = HttpMethod.Get,
                Data = null,
                Url = $"/api/Order",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response.ToResponseDtoWithCastedResult<IEnumerable<CustomerOrderDto>>();
        }

        public async Task<ResponseDto> SetShippedOrderStatus(string orderId)
        {
            var request = new RequestDto
            {
                HttpMethod = HttpMethod.Post,
                Data = null,
                Url = $"/api/Order/mark-as-shipped/{orderId}",
                IncludeAccessToken = true
            };

            ResponseDto response = await SendAsync(request);
            return response;
        }
    }
}