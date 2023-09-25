
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Repositories
{
    public class BuyerRepository : RepositoryBase<Buyer, int>, IBuyerRepository
    {
        public BuyerRepository(ApplicationDbContext db) : base(db)
        {
        }

        public async Task<Buyer?> GetByUserIdAsync(Guid userId)
        {
            Buyer? buyer = await _dbSet.FirstOrDefaultAsync(o => o.UserId == userId);
            return buyer;
        }
    }
}