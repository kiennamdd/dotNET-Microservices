
using Catalog.API.Domain.Entities;

namespace Catalog.API.Interfaces
{
    public interface IProductRepository: IRepositoryBase<Product, Guid>
    {
        
    }
}