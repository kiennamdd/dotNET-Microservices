using Discount.API.Domain.Entities;
using MongoDB.Driver;

namespace Discount.API.Interfaces
{
    public interface IApplicationDbContext
    {
        IMongoCollection<Coupon> Coupons { get; protected set; }
        IMongoCollection<T> GetCollection<T>();
    }
}
