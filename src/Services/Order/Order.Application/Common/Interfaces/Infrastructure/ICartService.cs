
using Order.Application.Common.Models;

namespace Order.Application.Common.Interfaces.Infrastructure
{
    public interface ICartService
    {
        Task<ShoppingCartDto?> GetCartByUserId(Guid userId);
    }
}