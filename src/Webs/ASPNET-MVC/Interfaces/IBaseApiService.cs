using ASPNET_MVC.Models;
using Cart.API.Models;

namespace ASPNET_MVC.Interfaces
{
    public interface IBaseApiService
    {
        Task<ResponseDto> SendAsync(RequestDto requestDto);
    }
}