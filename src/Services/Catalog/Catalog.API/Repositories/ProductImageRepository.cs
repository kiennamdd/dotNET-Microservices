using Catalog.API.Data;
using Catalog.API.Domain.Entities;
using Catalog.API.Interfaces;

namespace Catalog.API.Repositories
{
    public class ProductImageRepository: RepositoryBase<ProductImage, Guid>, IProductImageRepository
    {
        public ProductImageRepository(ApplicationDbContext db)
            :base(db)
        {
            
        }
    }
}