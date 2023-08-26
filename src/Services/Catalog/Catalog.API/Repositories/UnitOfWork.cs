using Catalog.API.Data;
using Catalog.API.Interfaces;

namespace Catalog.API.Repositories
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;    
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}