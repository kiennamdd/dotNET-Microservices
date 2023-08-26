using Catalog.API.Domain.Entities;

namespace Catalog.API.Interfaces
{
    public interface IBrandRepository: IRepositoryBase<Brand, int>
    {
        Task<Brand?> GetByNameAsync(string brandName);
    }
}