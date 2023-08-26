using Catalog.API.Data;
using Catalog.API.Domain.Entities;
using Catalog.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Repositories
{
    public class CategoryRepository: RepositoryBase<Category, int>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext db)
            :base(db)
        {
            
        }

        public async Task<Category?> GetByNameAsync(string categoryName)
        {
            return await _dbSet.FirstOrDefaultAsync(o => o.Name.Equals(categoryName));
        }
    }
}