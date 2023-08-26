
using Catalog.API.Data;
using Catalog.API.Domain.Entities;
using Catalog.API.Interfaces;

namespace Catalog.API.Repositories
{
    public class ProductRepository: RepositoryBase<Product, Guid>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext db)
            :base(db)
        {
            
        }
    }
}