
using Cart.API.Models;

namespace Cart.API.Interfaces.Infrastructure
{
    public interface ICatalogService
    {
        Task<ProductDto?> GetProductByIdAsync(Guid productId);
    }
}