using ASPNET_MVC.Models.Identity;
using Cart.API.Models;

namespace ASPNET_MVC.Interfaces
{
    public interface IIdentityService: IBaseApiService
    {
        Task<ResponseDto> Register(RegisterRequest registerRequest);
        Task<ResponseDto<SignInResponse>> SignIn(SignInRequest signInRequest);
        Task<ResponseDto> AssignRole(string userId, string roleName);
        Task<ResponseDto> DeleteUser(string userId);
    }
}