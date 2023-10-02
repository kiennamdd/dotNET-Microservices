using ASPNET_MVC.Models.Order;
using Cart.API.Models;

namespace ASPNET_MVC.Interfaces
{
    public interface IOrderService: IBaseApiService
    {
        Task<ResponseDto<CreateOrderResponse>> CreateOrder(CreateOrderRequest createOrderRequest);
        Task<ResponseDto<CheckoutResponse>> CreateCheckout(CheckoutRequest checkoutRequest);
        Task<ResponseDto> ConfirmCheckout(string orderId);
        Task<ResponseDto> CancelOrder(string orderId);
        Task<ResponseDto> SetShippedOrderStatus(string orderId);
        Task<ResponseDto<IEnumerable<CustomerOrderDto>>> GetOrderHistory();
        Task<ResponseDto<IEnumerable<CustomerOrderDto>>> GetOrderList();
        Task<ResponseDto<CustomerOrderDto>> GetOrderDetails(string orderId);
    }
}