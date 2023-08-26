using Catalog.API.Domain.Entities;

namespace Catalog.API.Interfaces
{
    public interface ICategoryRepository: IRepositoryBase<Category, int>
    {
        Task<Category?> GetByNameAsync(string categoryName);
    }
}