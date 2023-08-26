using Catalog.API.Data;
using Catalog.API.Domain.Entities;
using Catalog.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Repositories
{
    public class BrandRepository: RepositoryBase<Brand, int>, IBrandRepository
    {
        public BrandRepository(ApplicationDbContext db)
            :base(db)
        {
            
        }

        public async Task<Brand?> GetByNameAsync(string brandName)
        {
            return await _dbSet.FirstOrDefaultAsync(o => o.Name.Equals(brandName));
        }
    }
}