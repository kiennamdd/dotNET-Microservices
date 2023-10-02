using ASPNET_MVC.Models.Cart;
using Cart.API.Models;

namespace ASPNET_MVC.Interfaces
{
    public interface ICartService: IBaseApiService
    {
        Task<ResponseDto<ShoppingCartDto>> GetCartDetails(string userId);
        Task<ResponseDto> UpsertCartItem(CartItemUpsertRequest upsertRequest);
        Task<ResponseDto> RemoveCartItem(int cartItemId);
        Task<ResponseDto> ApplyCouponForCart(string couponCode);        
    }
}