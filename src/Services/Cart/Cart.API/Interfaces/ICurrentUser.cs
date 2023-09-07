
namespace Cart.API.Interfaces
{
    public interface ICurrentUser
    {
        Guid GetUserId();
        bool IsMatchCurrentUserId(string userIdToCheck, out Guid userIdResult);
        bool IsInRole(string role);
    }
}