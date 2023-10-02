namespace ASPNET_MVC.Interfaces
{
    public interface ITokenProvider
    {
        void SetAccessToken(string token);
        string? GetAccessToken();
        void ClearToken();
    }
}