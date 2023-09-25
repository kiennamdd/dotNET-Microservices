
namespace Order.Application.Common.Interfaces
{
    public interface ICurrentUser
    {
        Guid GetUserId();
        string GetUserName();
        bool IsMatchCurrentUserId(string userIdToCheck, out Guid userIdResult);
        bool IsInRole(string role);
    }
}