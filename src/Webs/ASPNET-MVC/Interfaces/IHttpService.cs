using ASPNET_MVC.Models;
using Cart.API.Models;

namespace ASPNET_MVC.Interfaces
{
    public interface IHttpService
    {
        HttpClient GetClient(string clientName, string baseUrl = "");
        Task<ResponseDto> SendAsync(RequestDto requestDto, HttpClient client);
    }
}