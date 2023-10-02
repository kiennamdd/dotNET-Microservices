using ASPNET_MVC.Constants;
using ASPNET_MVC.Interfaces;
using ASPNET_MVC.Models;
using ASPNET_MVC.Models.Identity;
using Cart.API.Models;

namespace ASPNET_MVC.Services
{
    public class IdentityService : BaseApiService, IIdentityService
    {
        public IdentityService(IHttpService httpService) : base(httpService)
        {
            Client = httpService.GetClient(ApiServiceNames.IdentityApi);
        }

        public Task<ResponseDto> AssignRole(string userId, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDto> DeleteUser(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto> Register(RegisterRequest registerRequest)
        {
            var request = new RequestDto
            {
                HttpMethod = HttpMethod.Post,
                Data = registerRequest,
                Url = "/api/Identity/Register",
                IncludeAccessToken = false
            };

            ResponseDto response = await SendAsync(request);
            return response;
        }

        public async Task<ResponseDto<SignInResponse>> SignIn(SignInRequest signInRequest)
        {
            var request = new RequestDto
            {
                HttpMethod = HttpMethod.Post,
                Data = signInRequest,
                Url = "/api/Identity/SignIn",
                IncludeAccessToken = false
            };

            ResponseDto response = await SendAsync(request);
            return response.ToResponseDtoWithCastedResult<SignInResponse>();
        }
    }
}