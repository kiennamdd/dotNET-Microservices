using ASPNET_MVC.Interfaces;
using ASPNET_MVC.Models;
using Cart.API.Models;

namespace ASPNET_MVC.Services
{
    public class BaseApiService : IBaseApiService
    {
        private readonly IHttpService _httpService;
        protected HttpClient? Client { get; set; } = null;

        public BaseApiService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ResponseDto> SendAsync(RequestDto requestDto)
        {
            if(Client is null)
                throw new ArgumentNullException(nameof(Client));

            return await _httpService.SendAsync(requestDto, Client);
        }
    }
}