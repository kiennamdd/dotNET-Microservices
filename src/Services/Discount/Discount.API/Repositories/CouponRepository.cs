using Discount.API.Domain.Entities;
using Discount.API.Interfaces;
using MongoDB.Driver;

namespace Discount.API.Repositories
{
    public class CouponRepository : RepositoryBase<Coupon, string>, ICouponRepository
    {
        public CouponRepository(IApplicationDbContext db)
            :base(db)
        {
            //
        }

        public async Task<Coupon?> GetByCodeAsync(string code)
        {
            var filter = Builders<Coupon>.Filter.Eq(o => o.CouponCode, code);
            var coupon = await _collection.Find(filter).FirstOrDefaultAsync();
            return coupon;
        }
    }
}
